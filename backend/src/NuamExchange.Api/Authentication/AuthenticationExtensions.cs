using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuamExchange.Infrastructure.Authentication;

namespace NuamExchange.Api.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtBearerAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services.AddAuthorization();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        return services;
    }
}

internal sealed class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions) : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name is not null && name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        var validatedOptions = Validate(jwtOptions.Value);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(validatedOptions.SigningKey!));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = validatedOptions.Issuer,
            ValidAudience = validatedOptions.Audience,
            IssuerSigningKey = signingKey,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(JwtBearerDefaults.AuthenticationScheme, options);

    private static JwtOptions Validate(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            throw new InvalidOperationException("Jwt:Issuer must be configured and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException("Jwt:Audience must be configured and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey must be configured and cannot be empty.");
        }

        if (options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey must contain at least 32 characters.");
        }

        if (options.AccessTokenMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:AccessTokenMinutes must be greater than zero.");
        }

        return options;
    }
}
