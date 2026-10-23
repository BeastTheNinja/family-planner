using Microsoft.EntityFrameworkCore;
using FamilyPlanner.Api.Models;

namespace FamilyPlanner.Api.Data;

public class FamilyPlannerDbContext : DbContext
{
    public FamilyPlannerDbContext(
        DbContextOptions<FamilyPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<FamilyMember> FamilyMembers { get; set; } = null!;
    public DbSet<FamilyTask> FamilyTasks {get; set;} = null!;
}