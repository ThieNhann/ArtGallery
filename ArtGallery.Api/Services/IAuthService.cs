using ArtGallery.Api.DTOs;

namespace ArtGallery.Api.Services;

public interface IAuthService
{
    Task<AuthRegistrationResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

public sealed record AuthRegistrationResult(
    AuthResponse? Response,
    IReadOnlyList<string> Errors,
    bool IsConflict);
