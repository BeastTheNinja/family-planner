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
                .Select(member => new FamilyMemberResponseDto

                {
                    Id = member.Id,
                    Name = member.Name,
                    IsActive = member.IsActive,
                    Tasks = member.Tasks.Select(task => new FamilyMemberTaskDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        IsCompleted = task.IsCompleted
                    }).ToList()
                })
                .ToListAsync();
            return Results.Ok(members);
        });

        app.MapGet("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id) =>
        {
            var familyMember = await db.FamilyMembers
                .Where(member => member.Id == id)
                 .Select(member => new FamilyMemberResponseDto

                 {
                     Id = member.Id,
                     Name = member.Name,
                     IsActive = member.IsActive,
                     Tasks = member.Tasks.Select(task => new FamilyMemberTaskDto
                     {
                         Id = task.Id,
                         Title = task.Title,
                         IsCompleted = task.IsCompleted
                     }).ToList()
                 })
                .FirstOrDefaultAsync();

            if (familyMember == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyMember not found",
                    detail: $"The requested FamilyMember with ID {id} was not found."
                );
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

            var response = new FamilyMemberResponseDto
            {
                Id = member.Id,
                Name = member.Name,
                IsActive = member.IsActive,
            };

            return Results.Created(
                $"/api/familymembers/{member.Id}",
                value: response
            );
        });

        app.MapPut("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id, UpdateFamilyMemberDto updatedFamilyMember) =>
        {
            var existingFamilyMember = await db.FamilyMembers.FindAsync(id);
            if (existingFamilyMember == null)
            {
                return Results.Problem(
                     statusCode: 404,
                    title: "FamilyMember not found",
                    detail: "The requested FamilyMember was not found."
                );
            }

            existingFamilyMember.Name = updatedFamilyMember.Name;
            existingFamilyMember.IsActive = updatedFamilyMember.IsActive;

            await db.SaveChangesAsync();

            var familyMember = await db.FamilyMembers
                .Where(member => member.Id == id)
                 .Select(member => new FamilyMemberResponseDto

                 {
                     Id = member.Id,
                     Name = member.Name,
                     IsActive = member.IsActive,
                     Tasks = member.Tasks.Select(task => new FamilyMemberTaskDto
                     {
                         Id = task.Id,
                         Title = task.Title,
                         IsCompleted = task.IsCompleted
                     }).ToList()
                 })
                .FirstOrDefaultAsync();

            if (familyMember == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyMember not found",
                    detail: $"The requested FamilyMember with ID {id} was not found."
                );
            }

            return Results.Ok(familyMember);
        });

        app.MapDelete("/api/familymembers/{id}", async (FamilyPlannerDbContext db, int id) =>
        {
            var deleteFamilyMember = await db.FamilyMembers.FindAsync(id);
            if (deleteFamilyMember == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyMember not found",
                    detail: $"The requested FamilyMember with ID {id} was not found."
                );
            }

            db.FamilyMembers.Remove(deleteFamilyMember);

            await db.SaveChangesAsync();

            return Results.NoContent();

        });
    }
}