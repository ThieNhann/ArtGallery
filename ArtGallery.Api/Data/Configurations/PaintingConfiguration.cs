using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtGallery.Api.Data.Configurations;

public sealed class PaintingConfiguration : IEntityTypeConfiguration<Painting>
{
    public void Configure(EntityTypeBuilder<Painting> builder)
    {
        builder.ToTable("Paintings", table =>
        {
            table.HasCheckConstraint("CK_Paintings_Price", "\"Price\" > 0");
            table.HasCheckConstraint("CK_Paintings_Dimensions", "\"WidthCm\" > 0 AND \"HeightCm\" > 0");
            table.HasCheckConstraint("CK_Paintings_Year", "\"Year\" IS NULL OR \"Year\" > 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(5000);
        builder.Property(x => x.Price).HasPrecision(12, 2);
        builder.Property(x => x.WidthCm).HasPrecision(8, 2);
        builder.Property(x => x.HeightCm).HasPrecision(8, 2);
        builder.Property(x => x.Medium).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.RowVersion).HasColumnName("xmin").IsRowVersion();

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Paintings)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
