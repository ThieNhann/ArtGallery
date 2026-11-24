using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtGallery.Api.Data.Configurations;

public sealed class PaintingImageConfiguration : IEntityTypeConfiguration<PaintingImage>
{
    public void Configure(EntityTypeBuilder<PaintingImage> builder)
    {
        builder.ToTable("PaintingImages", table =>
            table.HasCheckConstraint("CK_PaintingImages_Dimensions", "\"Width\" > 0 AND \"Height\" > 0"));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.StorageKey).HasMaxLength(512).IsRequired();
        builder.Property(x => x.ThumbUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.MediumUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.FullUrl).HasMaxLength(2048).IsRequired();
        builder.HasIndex(x => new { x.PaintingId, x.SortOrder }).IsUnique();

        builder.HasOne(x => x.Painting)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PaintingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
