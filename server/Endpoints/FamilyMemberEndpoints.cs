using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyPlanner.Api.Endpoints;

public static class FamilyMemberEndpoints
{
    public static void MapFamilyMemberEndpoints(this WebApplication app)
    {

        app.MapGet("/api/familymembers", async (FamilyPlannerDbContext db) =>
        {
            var members = await db.FamilyMembers.ToListAsync();
            return Results.Ok(members);
        });

        app.MapGet("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id) =>
        {
            var familyMember = await db.FamilyMembers.FindAsync(id);
            if (familyMember == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(familyMember);
        });

        app.MapPost("/api/familymembers", async (FamilyPlannerDbContext db, FamilyMember newFamilyMember) =>
        {


            db.FamilyMembers.Add(newFamilyMember);

            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/familymembers/{newFamilyMember.Id}",
                newFamilyMember
            );
        });

        app.MapPut("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id, FamilyMember updatedFamilyMember) =>
        {
            var existingFamilyMember = await db.FamilyMembers.FindAsync(id);
            if (existingFamilyMember == null)
            {
                return Results.NotFound();
            }

            existingFamilyMember.Name = updatedFamilyMember.Name;
            existingFamilyMember.IsActive = updatedFamilyMember.IsActive;

            await db.SaveChangesAsync();

            return Results.Ok(existingFamilyMember);
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