namespace NuamExchange.Application.Authentication;

public sealed record JwtTokenResult(string AccessToken, DateTimeOffset ExpiresAtUtc);
