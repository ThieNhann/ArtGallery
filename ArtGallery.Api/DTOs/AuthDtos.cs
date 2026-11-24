using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Api.DTOs;

/// <summary>Credentials and profile details for creating an account.</summary>
public sealed record RegisterRequest
{
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    [Required, MinLength(8), MaxLength(128)]
    public required string Password { get; init; }

    [Required, MaxLength(200)]
    public required string FullName { get; init; }
}

/// <summary>Credentials for signing in to an account.</summary>
public sealed record LoginRequest
{
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    [Required, MaxLength(128)]
    public required string Password { get; init; }
}

/// <summary>Access token and profile details returned after authentication.</summary>
public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc,
    string UserId,
    string Email,
    string FullName);
