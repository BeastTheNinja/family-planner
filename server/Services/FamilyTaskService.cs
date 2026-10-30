using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.DTOs;
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

        return await _db.FamilyTasks.Include(task => task.FamilyMember).ToListAsync();
    }
    public async Task<FamilyTask?> GetByIdAsync(int id)
    {
        return await _db.FamilyTasks
            .Include(task => task.FamilyMember)
            .FirstOrDefaultAsync(task => task.Id == id);
    }
    public async Task<bool> FamilyMemberExistsAsync(int id)
    {

        return await _db.FamilyMembers.AnyAsync(fm => fm.Id == id);
    }
    public async Task<FamilyTask> CreateAsync(CreateFamilyTaskDto newTask)
    {
        var familyTask = new FamilyTask
        {
            Title = newTask.Title,
            Description = newTask.Description,
            IsCompleted = false,
            FamilyMemberId = newTask.FamilyMemberId
        };
        _db.FamilyTasks.Add(familyTask);
        await _db.SaveChangesAsync();
        return familyTask;
    }
    public async Task<string?> GetFamilyMemberNameAsync(int? familyMemberId)
    {
        if (familyMemberId == null)
        {
            return null;
        }

        var familyMember = await _db.FamilyMembers.FindAsync(familyMemberId);
        return familyMember?.Name;
    }
    public async Task<FamilyTask> UpdateAsync(
    FamilyTask existingTask,
    UpdateFamilyTaskDto updatedTask)
    {

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.IsCompleted = updatedTask.IsCompleted;
        existingTask.FamilyMemberId = updatedTask.FamilyMemberId;

        await _db.SaveChangesAsync();
        return existingTask;
    }
    public async Task DeleteAsync(FamilyTask existingTask)
    {
        _db.FamilyTasks.Remove(existingTask);
        await _db.SaveChangesAsync();
    }
}
