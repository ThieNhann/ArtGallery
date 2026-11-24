using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Api.DTOs;

/// <summary>Filters and pagination options for the public painting catalog.</summary>
public sealed record PublicPaintingsQuery : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 12;

    [MaxLength(200)]
    public string? Search { get; init; }

    public Guid? CategoryId { get; init; }

    [MaxLength(120)]
    public string? Medium { get; init; }

    [EnumDataType(typeof(PublicPaintingOrder))]
    public PublicPaintingOrder Order { get; init; } = PublicPaintingOrder.Title;

    [Range(typeof(decimal), "0.01", "9999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? MinPrice { get; init; }

    [Range(typeof(decimal), "0.01", "9999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    public decimal? MaxPrice { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice > MaxPrice)
        {
            yield return new ValidationResult(
                "MinPrice must be less than or equal to MaxPrice.",
                [nameof(MinPrice), nameof(MaxPrice)]);
        }

        if (Page > 0 && PageSize > 0 && (long)(Page - 1) * PageSize > int.MaxValue)
        {
            yield return new ValidationResult(
                "The requested page offset is too large.",
                [nameof(Page)]);
        }
    }
}
