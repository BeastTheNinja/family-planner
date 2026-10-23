namespace FamilyPlanner.Api.Models;

public class FamilyTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int? FamilyMemberId { get; set; }
    public FamilyMember? FamilyMember { get; set; }
    
}