    using System.ComponentModel.DataAnnotations;

    namespace FamilyPlanner.Api.DTOs;

    public class CreateFamilyMemberDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }