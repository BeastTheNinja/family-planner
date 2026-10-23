using System.ComponentModel.DataAnnotations;

namespace FamilyPlanner.Api.DTOs;

public class FamilyTaskBaseDto : IValidatableObject
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must be between 1 and 100 characters.", MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    [StringLength(500, ErrorMessage = "Description must be between 1 and 500 characters.")]
    public string? Description { get; set; }
    public int? FamilyMemberId { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult("Title cannot be empty or whitespace.", new[] { nameof(Title) });
        }

        if (string.Equals(Title, "Test", StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult("Title cannot be 'Test'.", new[] { nameof(Title) });
        }

        if (string.Equals(Title, "Important", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(Description))
        {
            yield return new ValidationResult("Description is required when Title is Important.", new[] { nameof(Description) });
        }
    }
}