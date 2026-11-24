namespace ArtGallery.Api.Models;

public sealed class Cart
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string UserId { get; set; }
    public ICollection<CartItem> Items { get; set; } = [];
}
