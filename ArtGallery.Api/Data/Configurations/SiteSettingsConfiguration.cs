using ArtGallery.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtGallery.Api.Data.Configurations;

public sealed class SiteSettingsConfiguration : IEntityTypeConfiguration<SiteSettings>
{
    public void Configure(EntityTypeBuilder<SiteSettings> builder)
    {
        builder.ToTable("SiteSettings", table =>
        {
            table.HasCheckConstraint("CK_SiteSettings_Id", "\"Id\" = 1");
            table.HasCheckConstraint("CK_SiteSettings_FlatShippingFee", "\"FlatShippingFee\" >= 0");
            table.HasCheckConstraint("CK_SiteSettings_ReservationMinutes", "\"ReservationMinutes\" > 0");
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ShopName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.ArtistName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.ArtistBio).HasMaxLength(4000);
        builder.Property(x => x.ArtistPhotoUrl).HasMaxLength(2048);
        builder.Property(x => x.LogoUrl).HasMaxLength(2048);
        builder.Property(x => x.PrimaryColor).HasMaxLength(7);
        builder.Property(x => x.ContactEmail).HasMaxLength(254);
        builder.Property(x => x.BankAccountInfo).HasMaxLength(1000);
        builder.Property(x => x.ReturnPolicy).HasMaxLength(4000);
        builder.Property(x => x.FlatShippingFee).HasPrecision(12, 2);
        builder.Property(x => x.ReservationMinutes).HasDefaultValue(20);
    }
}
