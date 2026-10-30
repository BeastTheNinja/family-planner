using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FamilyPlanner.Api.Services;


public class FamilyMemberService
{
    private readonly FamilyPlannerDbContext _db;

    public FamilyMemberService(FamilyPlannerDbContext db)
    {
        _db = db;

    }
    public async Task<List<FamilyMember>> GetAllAsync()
    {
        return await _db.FamilyMembers.Include(member => member.Tasks)
            .ToListAsync();
    }
    public async Task<FamilyMember?> GetByIdAsync(int id)
    {
        return await _db.FamilyMembers
            .Include(member => member.Tasks)
            .FirstOrDefaultAsync(member => member.Id == id);


    }
    public async Task<FamilyMember> CreateAsync(CreateFamilyMemberDto newMember)
    {
        var familyMember = new FamilyMember
        {
            Name = newMember.Name,
            IsActive = true
        };
        _db.FamilyMembers.Add(familyMember);
        await _db.SaveChangesAsync();
        return familyMember;
    }
    public async Task<FamilyMember> UpdateAsync(
     FamilyMember existingMember,
     UpdateFamilyMemberDto updatedMember)
    {
        existingMember.Name = updatedMember.Name;
        existingMember.IsActive = updatedMember.IsActive;

        await _db.SaveChangesAsync();

        return existingMember;
    }
    public async Task DeleteAsync(FamilyMember existingMember)
    {
        _db.FamilyMembers.Remove(existingMember);
        await _db.SaveChangesAsync();
    }
}