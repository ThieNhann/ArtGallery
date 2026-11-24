namespace ArtGallery.Api.Models;

public sealed class PaintingImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaintingId { get; set; }
    public required string StorageKey { get; set; }
    public required string ThumbUrl { get; set; }
    public required string MediumUrl { get; set; }
    public required string FullUrl { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int SortOrder { get; set; }
    public Painting Painting { get; set; } = null!;
}
