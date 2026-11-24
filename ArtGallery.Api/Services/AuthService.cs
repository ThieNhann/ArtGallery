using ArtGallery.Api.DTOs;
using ArtGallery.Api.Identity;
using ArtGallery.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ArtGallery.Api.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService) : IAuthService
{
    public async Task<AuthRegistrationResult> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var email = request.Email.Trim();
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            LockoutEnabled = true
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var isConflict = createResult.Errors.Any(error =>
                error.Code is "DuplicateEmail" or "DuplicateUserName");
            return new AuthRegistrationResult(null, createResult.Errors.Select(error => error.Description).ToArray(), isConflict);
        }

        var roleResult = await userManager.AddToRoleAsync(user, ApplicationRoles.Customer);
        if (!roleResult.Succeeded)
        {
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var errors = roleResult.Errors.Concat(deleteResult.Errors)
                    .Select(error => error.Description)
                    .ToArray();
                throw new InvalidOperationException(
                    $"Failed to assign the customer role and remove the incomplete account: {string.Join("; ", errors)}");
            }

            return new AuthRegistrationResult(null, roleResult.Errors.Select(error => error.Description).ToArray(), false);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var roles = await userManager.GetRolesAsync(user);
        return new AuthRegistrationResult(tokenService.CreateToken(user, roles), [], false);
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return null;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var roles = await userManager.GetRolesAsync(user);
        return tokenService.CreateToken(user, roles);
    }
}
