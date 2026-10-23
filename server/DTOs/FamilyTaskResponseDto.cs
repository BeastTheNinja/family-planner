namespace FamilyPlanner.Api.DTOs;

public class FamilyTaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int? FamilyMemberId { get; set; }
    public string? FamilyMemberName { get; set; }
}