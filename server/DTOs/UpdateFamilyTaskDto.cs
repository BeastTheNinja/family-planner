using System.ComponentModel.DataAnnotations;

namespace FamilyPlanner.Api.DTOs;


public class UpdateFamilyTaskDto
{
    [Required]
    [StringLength(100)]
    public string  Title { get; set; } = string.Empty;
    [StringLength(500)]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int? FamilyMemberId { get; set; }
}