using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtGallery.Api.Data.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", table =>
            table.HasCheckConstraint("CK_Orders_Total", "\"Total\" > 0"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).HasMaxLength(450).IsRequired();
        builder.Property(x => x.Total).HasPrecision(12, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.PaymentStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.ShippingAddress).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.ContactName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.ContactEmail).HasMaxLength(254).IsRequired();
        builder.Property(x => x.ContactPhone).HasMaxLength(32).IsRequired();
    }
}
