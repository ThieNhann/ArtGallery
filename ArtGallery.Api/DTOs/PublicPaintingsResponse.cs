namespace ArtGallery.Api.DTOs;

/// <summary>A page of paintings available in the public catalog.</summary>
public sealed record PublicPaintingsResponse(
    IReadOnlyList<PublicPaintingResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

/// <summary>A painting and its public catalog details.</summary>
public sealed record PublicPaintingResponse(
    Guid Id,
    string Title,
    string? Description,
    decimal Price,
    decimal WidthCm,
    decimal HeightCm,
    string Medium,
    int? Year,
    bool IsFramed,
    string? CategoryName,
    string? ThumbnailUrl);
