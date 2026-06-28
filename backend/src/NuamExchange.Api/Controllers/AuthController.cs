using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuamExchange.Api.Authentication;

namespace NuamExchange.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserResponse> Me()
    {
        var userId = FindFirstValue(JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);
        var email = FindFirstValue(JwtRegisteredClaimNames.Email, ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(email))
        {
            return Problem(
                title: "Authenticated token is missing required user claims.",
                detail: "The authenticated token must contain both userId and email claims.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        var fullName = FindFirstValue(JwtRegisteredClaimNames.Name, ClaimTypes.Name) ?? User.Identity?.Name;
        var roles = User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value.Trim())
            .Where(role => role.Length > 0)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        return Ok(new CurrentUserResponse(userId, email, fullName, roles));
    }

    private string? FindFirstValue(params string[] claimTypes) =>
        claimTypes.Select(claimType => User.FindFirst(claimType)?.Value)
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
}
