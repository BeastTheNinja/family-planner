namespace FamilyPlanner.Api.DTOs;

public class FamilyMemberResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<FamilyMemberTaskDto> Tasks { get; set; } = new();
}