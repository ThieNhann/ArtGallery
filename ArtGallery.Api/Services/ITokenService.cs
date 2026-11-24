using ArtGallery.Api.DTOs;
using ArtGallery.Api.Models;

namespace ArtGallery.Api.Services;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user, IEnumerable<string> roles);
}
