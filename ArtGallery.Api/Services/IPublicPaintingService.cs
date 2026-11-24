using ArtGallery.Api.DTOs;

namespace ArtGallery.Api.Services;

public interface IPublicPaintingService
{
    Task<PublicPaintingsResponse> GetAsync(
        PublicPaintingsQuery query,
        CancellationToken cancellationToken);
}
