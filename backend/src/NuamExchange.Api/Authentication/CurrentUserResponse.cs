namespace NuamExchange.Api.Authentication;

public sealed record CurrentUserResponse(
    string UserId,
    string Email,
    string? FullName,
    IReadOnlyCollection<string> Roles);
