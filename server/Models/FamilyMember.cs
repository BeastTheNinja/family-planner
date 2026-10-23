namespace FamilyPlanner.Api.Models;

public class FamilyMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<FamilyTask> Tasks { get; set; } = new();
}