using ArtGallery.Api.DTOs;
using ArtGallery.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtGallery.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Registers a customer account and returns an access token.</summary>
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        if (result.Response is not null)
        {
            return Ok(result.Response);
        }

        if (result.IsConflict)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Account already exists",
                Detail = "An account with this email address already exists.",
                Status = StatusCodes.Status409Conflict
            });
        }

        return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
        {
            ["registration"] = result.Errors.ToArray()
        }));
    }

    /// <summary>Authenticates a user and returns an access token.</summary>
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        if (response is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = "Email or password is incorrect.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        return Ok(response);
    }
}
