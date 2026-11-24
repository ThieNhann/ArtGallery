using ArtGallery.Api.Data;
using ArtGallery.Api.DTOs;
using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Api.Services;

public sealed class PublicPaintingService(ArtGalleryDbContext dbContext) : IPublicPaintingService
{
    public async Task<PublicPaintingsResponse> GetAsync(
        PublicPaintingsQuery query,
        CancellationToken cancellationToken)
    {
        IQueryable<Painting> paintings = dbContext.Paintings
            .AsNoTracking()
            .Where(painting => painting.Status == PaintingStatus.Available);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = $"%{query.Search.Trim()}%";
            paintings = paintings.Where(painting =>
                EF.Functions.ILike(painting.Title, search) ||
                (painting.Description != null && EF.Functions.ILike(painting.Description, search)));
        }

        if (query.CategoryId is Guid categoryId)
        {
            paintings = paintings.Where(painting => painting.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(query.Medium))
        {
            var medium = $"%{query.Medium.Trim()}%";
            paintings = paintings.Where(painting => EF.Functions.ILike(painting.Medium, medium));
        }

        if (query.MinPrice is decimal minPrice)
        {
            paintings = paintings.Where(painting => painting.Price >= minPrice);
        }

        if (query.MaxPrice is decimal maxPrice)
        {
            paintings = paintings.Where(painting => painting.Price <= maxPrice);
        }

        var totalCount = await paintings.CountAsync(cancellationToken);
        var orderedPaintings = query.Order switch
        {
            PublicPaintingOrder.PriceAsc => paintings
                .OrderBy(painting => painting.Price)
                .ThenBy(painting => painting.Title),
            PublicPaintingOrder.PriceDesc => paintings
                .OrderByDescending(painting => painting.Price)
                .ThenBy(painting => painting.Title),
            PublicPaintingOrder.YearAsc => paintings
                .OrderBy(painting => painting.Year)
                .ThenBy(painting => painting.Title),
            PublicPaintingOrder.YearDesc => paintings
                .OrderByDescending(painting => painting.Year)
                .ThenBy(painting => painting.Title),
            _ => paintings.OrderBy(painting => painting.Title)
        };
        var items = await orderedPaintings
            .ThenBy(painting => painting.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(painting => new PublicPaintingResponse(
                painting.Id,
                painting.Title,
                painting.Description,
                painting.Price,
                painting.WidthCm,
                painting.HeightCm,
                painting.Medium,
                painting.Year,
                painting.IsFramed,
                painting.Category == null ? null : painting.Category.Name,
                painting.Images
                    .OrderBy(image => image.SortOrder)
                    .Select(image => image.ThumbUrl)
                    .FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PublicPaintingsResponse(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)query.PageSize));
    }
}
