namespace NuamExchange.Application.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(JwtTokenRequest request);
}
