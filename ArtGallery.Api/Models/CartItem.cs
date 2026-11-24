namespace ArtGallery.Api.Models;

public sealed class CartItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CartId { get; set; }
    public Guid PaintingId { get; set; }
    public Cart Cart { get; set; } = null!;
    public Painting Painting { get; set; } = null!;
}
