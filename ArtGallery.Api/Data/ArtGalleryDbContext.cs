using ArtGallery.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Api.Data;

public sealed class ArtGalleryDbContext(DbContextOptions<ArtGalleryDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<SiteSettings> SiteSettings => Set<SiteSettings>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Painting> Paintings => Set<Painting>();
    public DbSet<PaintingImage> PaintingImages => Set<PaintingImage>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtGalleryDbContext).Assembly);
        modelBuilder.Entity<ApplicationUser>().Property(user => user.FullName).HasMaxLength(200).IsRequired();
    }
}
