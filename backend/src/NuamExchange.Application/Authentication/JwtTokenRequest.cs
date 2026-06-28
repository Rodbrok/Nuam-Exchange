namespace NuamExchange.Application.Authentication;

public sealed record JwtTokenRequest(
    string UserId,
    string Email,
    string? FullName,
    IReadOnlyCollection<string> Roles);
