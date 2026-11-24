using ArtGallery.Api.Models;
using ArtGallery.Api.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArtGallery.Api.Data.Extensions;

public static class DatabaseSeedExtensions
{
    private static readonly Guid AbstractCategoryId = Guid.Parse("11111111-1111-4111-8111-111111111111");
    private static readonly Guid LandscapeCategoryId = Guid.Parse("22222222-2222-4222-8222-222222222222");
    private static readonly Guid StillLifeCategoryId = Guid.Parse("33333333-3333-4333-8333-333333333333");

    public static IServiceCollection AddDatabaseSeedData(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseSeedHostedService>();
        return services;
    }

    public static async Task SeedDataAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var dbContext = services.GetRequiredService<ArtGalleryDbContext>();

        if (!await dbContext.SiteSettings.AnyAsync(x => x.Id == 1, cancellationToken))
        {
            dbContext.SiteSettings.Add(new SiteSettings
            {
                Id = 1,
                ShopName = "Art Gallery",
                ArtistName = "Demo Artist",
                ArtistBio = "A sample collection of original paintings created for the Art Gallery demo.",
                ContactEmail = "artist@example.com",
                ReturnPolicy = "Contact the gallery within seven days of delivery to discuss a return.",
                FlatShippingFee = 30000m,
                ReservationMinutes = 20
            });
        }

        var categories = new[]
        {
            new Category { Id = AbstractCategoryId, Name = "Abstract", Slug = "abstract" },
            new Category { Id = LandscapeCategoryId, Name = "Landscapes", Slug = "landscapes" },
            new Category { Id = StillLifeCategoryId, Name = "Still Life", Slug = "still-life" }
        };
        var categoryIds = categories.Select(x => x.Id).ToArray();
        var existingCategoryIds = await dbContext.Categories
            .Where(x => categoryIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);

        dbContext.Categories.AddRange(categories.Where(x => !existingCategoryIds.Contains(x.Id)));

        var paintings = new[]
        {
            new Painting
            {
                Id = Guid.Parse("44444444-4444-4444-8444-444444444444"),
                Title = "Quiet Horizon",
                Description = "Soft evening colors settle over a distant coastline.",
                Price = 12500000m,
                WidthCm = 60,
                HeightCm = 80,
                Medium = "Oil on canvas",
                Year = 2024,
                IsFramed = true,
                CategoryId = LandscapeCategoryId
            },
            new Painting
            {
                Id = Guid.Parse("55555555-5555-4555-8555-555555555555"),
                Title = "Blue Geometry",
                Description = "Layered blue forms explore balance, movement, and light.",
                Price = 9800000m,
                WidthCm = 50,
                HeightCm = 50,
                Medium = "Acrylic on canvas",
                Year = 2023,
                IsFramed = false,
                CategoryId = AbstractCategoryId
            },
            new Painting
            {
                Id = Guid.Parse("66666666-6666-4666-8666-666666666666"),
                Title = "Afternoon Fruit",
                Description = "A warm still life arranged in natural afternoon light.",
                Price = 7600000m,
                WidthCm = 40,
                HeightCm = 60,
                Medium = "Oil on canvas",
                Year = 2022,
                IsFramed = true,
                CategoryId = StillLifeCategoryId
            }
        };
        var paintingIds = paintings.Select(x => x.Id).ToArray();
        var existingPaintingIds = await dbContext.Paintings
            .Where(x => paintingIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);

        dbContext.Paintings.AddRange(paintings.Where(x => !existingPaintingIds.Contains(x.Id)));

        await dbContext.SaveChangesAsync(cancellationToken);
        await SeedIdentityAsync(services, cancellationToken);
    }

    private static async Task SeedIdentityAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var configuration = services.GetRequiredService<IConfiguration>();
        var configuredEmail = configuration["AdminSeed:Email"];
        if (string.IsNullOrWhiteSpace(configuredEmail))
        {
            throw new InvalidOperationException(
                "Configure AdminSeed:Email using user secrets or environment variables.");
        }

        var email = configuredEmail.Trim();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var roleName in ApplicationRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create the {roleName} role: {string.Join("; ", roleResult.Errors.Select(error => error.Description))}");
            }
        }

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            var password = configuration["AdminSeed:Password"];
            var fullName = configuration["AdminSeed:FullName"] ?? "Gallery Administrator";
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Configure AdminSeed:Password using user secrets or environment variables to create the admin account.");
            }

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 200)
            {
                throw new InvalidOperationException("AdminSeed:FullName must be between 1 and 200 characters.");
            }

            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName.Trim(),
                LockoutEnabled = true
            };

            var createResult = await userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed the admin account: {string.Join("; ", createResult.Errors.Select(error => error.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, ApplicationRoles.Admin))
        {
            var addRoleResult = await userManager.AddToRoleAsync(admin, ApplicationRoles.Admin);
            if (!addRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to assign the Admin role: {string.Join("; ", addRoleResult.Errors.Select(error => error.Description))}");
            }
        }
    }
}
