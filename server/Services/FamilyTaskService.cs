using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace FamilyPlanner.Api.Services;

public class FamilyTaskService
{
    private readonly FamilyPlannerDbContext _db;

    public FamilyTaskService(FamilyPlannerDbContext db)
    {
        _db = db;

    }
    public async Task<List<FamilyTask>> GetAllAsync()
    {
        // Din kode her
        return await _db.FamilyTasks.Include(task => task.FamilyMember).ToListAsync();
    }
    public async Task<FamilyTask?> GetByIdAsync(int id)
    {
        // Din kode her
        return await _db.FamilyTasks
            .Include(task => task.FamilyMember)
            .FirstOrDefaultAsync(task => task.Id == id);
    }

}
