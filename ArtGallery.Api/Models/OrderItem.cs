namespace ArtGallery.Api.Models;

public sealed class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public Guid? PaintingId { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public Order Order { get; set; } = null!;
    public Painting? Painting { get; set; }
}
