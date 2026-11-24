using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ArtGallery.Api.Models;

public sealed class ApplicationUser : IdentityUser
{
    [PersonalData]
    [MaxLength(200)]
    public required string FullName { get; set; }
}
