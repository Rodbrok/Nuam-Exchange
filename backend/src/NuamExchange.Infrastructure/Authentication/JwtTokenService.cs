using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuamExchange.Application.Authentication;

namespace NuamExchange.Infrastructure.Authentication;

public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenService
{
    private const int MinimumSigningKeyLength = 32;

    public JwtTokenResult GenerateToken(JwtTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var jwtOptions = ValidateOptions(options.Value);
        var issuedAtUtc = timeProvider.GetUtcNow();
        var expiresAtUtc = issuedAtUtc.AddMinutes(jwtOptions.AccessTokenMinutes);
        var signingCredentials = CreateSigningCredentials(jwtOptions.SigningKey!);

        var claims = CreateClaims(request, issuedAtUtc);
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            notBefore: issuedAtUtc.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: signingCredentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtTokenResult(accessToken, expiresAtUtc);
    }

    private static JwtOptions ValidateOptions(JwtOptions jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
        {
            throw new InvalidOperationException("JWT issuer must be configured.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
        {
            throw new InvalidOperationException("JWT audience must be configured.");
        }

        if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key must be configured.");
        }

        if (jwtOptions.SigningKey.Length < MinimumSigningKeyLength)
        {
            throw new InvalidOperationException($"JWT signing key must contain at least {MinimumSigningKeyLength} characters.");
        }

        if (jwtOptions.AccessTokenMinutes <= 0)
        {
            throw new InvalidOperationException("JWT access token expiration must be greater than zero minutes.");
        }

        return jwtOptions;
    }

    private static SigningCredentials CreateSigningCredentials(string signingKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> CreateClaims(JwtTokenRequest request, DateTimeOffset issuedAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, request.UserId),
            new(ClaimTypes.NameIdentifier, request.UserId),
            new(JwtRegisteredClaimNames.Email, request.Email),
            new(ClaimTypes.Email, request.Email),
            new(JwtRegisteredClaimNames.Iat, issuedAtUtc.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        };

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, request.FullName));
            claims.Add(new Claim(ClaimTypes.Name, request.FullName));
        }

        foreach (var role in request.Roles.Where(role => !string.IsNullOrWhiteSpace(role)))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}
