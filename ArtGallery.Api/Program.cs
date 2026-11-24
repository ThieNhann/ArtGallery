using System.Security.Claims;
using System.Text;
using ArtGallery.Api.Data;
using ArtGallery.Api.Data.Extensions;
using ArtGallery.Api.Models;
using ArtGallery.Api.Services;
using ArtGallery.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ArtGalleryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ArtGalleryDbContext>()
    .AddSignInManager();

builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)
    .ValidateDataAnnotations()
    .Validate(settings => Encoding.UTF8.GetByteCount(settings.Secret) >= 32,
        "Jwt:Secret must be at least 32 bytes.")
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration[$"{JwtSettings.SectionName}:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration[$"{JwtSettings.SectionName}:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (_, _, _, _) =>
            {
                var secret = builder.Configuration[$"{JwtSettings.SectionName}:Secret"];
                if (string.IsNullOrWhiteSpace(secret))
                {
                    throw new InvalidOperationException(
                        "Configure Jwt:Secret using user secrets or environment variables.");
                }

                return [new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))];
            },
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPublicPaintingService, PublicPaintingService>();
builder.Services.AddDatabaseSeedData();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
