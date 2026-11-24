namespace ArtGallery.Api.Models;

public sealed class Painting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public required string Medium { get; set; }
    public int? Year { get; set; }
    public bool IsFramed { get; set; }
    public PaintingStatus Status { get; set; } = PaintingStatus.Available;
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public uint RowVersion { get; private set; }
    public ICollection<PaintingImage> Images { get; set; } = [];
}
