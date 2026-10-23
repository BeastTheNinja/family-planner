using System.ComponentModel.DataAnnotations;

namespace FamilyPlanner.Api.DTOs;


public class CreateFamilyTaskDto
{

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
    [StringLength(500)]
    public string? Description { get; set; }
    public int? FamilyMemberId { get; set; }
}