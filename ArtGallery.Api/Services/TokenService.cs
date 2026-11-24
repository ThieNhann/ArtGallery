using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArtGallery.Api.DTOs;
using ArtGallery.Api.Models;
using ArtGallery.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ArtGallery.Api.Services;

public sealed class TokenService(IOptions<JwtSettings> options) : ITokenService
{
    public AuthResponse CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var settings = options.Value;
        var email = user.Email ?? throw new InvalidOperationException("Cannot create a token for a user without an email.");
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(settings.AccessTokenLifetimeMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Name, user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            "Bearer",
            expiresAt,
            user.Id,
            email,
            user.FullName);
    }
}
