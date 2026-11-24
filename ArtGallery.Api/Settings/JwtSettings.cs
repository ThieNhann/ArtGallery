using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Api.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    public string Secret { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenLifetimeMinutes { get; init; } = 60;
}
