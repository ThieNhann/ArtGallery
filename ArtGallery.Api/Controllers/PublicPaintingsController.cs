using ArtGallery.Api.DTOs;
using ArtGallery.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtGallery.Api.Controllers;

[ApiController]
[Route("api/public/paintings")]
[AllowAnonymous]
public sealed class PublicPaintingsController(IPublicPaintingService publicPaintingService) : ControllerBase
{
    /// <summary>Gets available paintings with optional filters and pagination.</summary>
    [HttpGet]
    [EndpointSummary("List available paintings")]
    [EndpointDescription("Returns a paginated catalog of available paintings with optional search, category, medium, and price filters.")]
    [ProducesResponseType<PublicPaintingsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicPaintingsResponse>> Get(
        [FromQuery] PublicPaintingsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await publicPaintingService.GetAsync(query, cancellationToken);
        return Ok(result);
    }
}
