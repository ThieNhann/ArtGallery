using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtGallery.Api.Data.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems", table =>
            table.HasCheckConstraint("CK_OrderItems_PriceAtPurchase", "\"PriceAtPurchase\" > 0"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.PriceAtPurchase).HasPrecision(12, 2);
        builder.HasIndex(x => new { x.OrderId, x.PaintingId }).IsUnique();

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Painting)
            .WithMany()
            .HasForeignKey(x => x.PaintingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
