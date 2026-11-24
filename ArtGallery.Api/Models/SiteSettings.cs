namespace ArtGallery.Api.Models;

public sealed class SiteSettings
{
    public int Id { get; set; } = 1;
    public required string ShopName { get; set; }
    public required string ArtistName { get; set; }
    public string? ArtistBio { get; set; }
    public string? ArtistPhotoUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? ContactEmail { get; set; }
    public string? BankAccountInfo { get; set; }
    public string? ReturnPolicy { get; set; }
    public decimal FlatShippingFee { get; set; }
    public int ReservationMinutes { get; set; } = 20;
}
