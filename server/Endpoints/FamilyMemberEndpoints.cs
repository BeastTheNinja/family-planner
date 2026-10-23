using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FamilyPlanner.Api.Endpoints;

public static class FamilyMemberEndpoints
{
    public static void MapFamilyMemberEndpoints(this WebApplication app)
    {

        app.MapGet("/api/familymembers", async (FamilyPlannerDbContext db) =>
        {
            var members = await db.FamilyMembers
                .Select(member => new

                {
                    member.Id,
                    member.Name,
                    member.IsActive,
                    Tasks = member.Tasks.Select(task => new
                    {
                        task.Id,
                        task.Title,
                        task.IsCompleted
                    }).ToList()
                })
                .ToListAsync();
            return Results.Ok(members);
        });

        app.MapGet("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id) =>
        {
            var familyMember = await db.FamilyMembers
                .Where(member => member.Id == id)
                .Select(member => new
                {
                    member.Id,
                    member.Name,
                    member.IsActive,
                    Tasks = member.Tasks.Select(task => new
                    {
                        task.Id,
                        task.Title,
                        task.IsCompleted
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (familyMember == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(familyMember);
        });

        app.MapPost("/api/familymembers", async (FamilyPlannerDbContext db, CreateFamilyMemberDto newMember) =>
        {
            var member = new FamilyMember
            {
                Name = newMember.Name,
                IsActive = newMember.IsActive
            };

            db.FamilyMembers.Add(member);

            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/familymembers/{member.Id}",
                member
            );
        });

        app.MapPut("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id, UpdateFamilyMemberDto updatedFamilyMember) =>
        {
            var existingFamilyMember = await db.FamilyMembers.FindAsync(id);
            if (existingFamilyMember == null)
            {
                return Results.NotFound();
            }

            existingFamilyMember.Name = updatedFamilyMember.Name;
            existingFamilyMember.IsActive = updatedFamilyMember.IsActive;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id) =>
        {
            var deleteFamilyMember = await db.FamilyMembers.FindAsync(id);
            if (deleteFamilyMember == null)
            {
                return Results.NotFound();
            }

            db.FamilyMembers.Remove(deleteFamilyMember);

            await db.SaveChangesAsync();

            return Results.NoContent();

        });
    }
}