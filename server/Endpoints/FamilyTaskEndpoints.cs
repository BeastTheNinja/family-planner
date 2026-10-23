using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FamilyPlanner.Api.Endpoints;

public static class FamilyTaskEndpoints
{
    public static void MapFamilyTaskEndpoints(this WebApplication app)
    {
        app.MapGet("/api/familytasks", async (FamilyPlannerDbContext db) =>
        {
            var tasks = await db.FamilyTasks
                .Select(task => new FamilyTaskResponseDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    FamilyMemberId = task.FamilyMemberId,
                    FamilyMemberName = task.FamilyMember != null
                ? task.FamilyMember.Name
                : null
                })
                .ToListAsync();
            return Results.Ok(tasks);
        });

        app.MapGet("/api/familytasks/{id}", async (int id, FamilyPlannerDbContext db) =>
        {

            var existingTask = await db.FamilyTasks
                .Include(task => task.FamilyMember)
                .FirstOrDefaultAsync(task => task.Id == id);


            if (existingTask == null)
            {
                return Results.NotFound();
            }


            return Results.Ok(new FamilyTaskResponseDto
            {
                Id = existingTask.Id,
                Title = existingTask.Title,
                Description = existingTask.Description,
                IsCompleted = existingTask.IsCompleted,
                FamilyMemberId = existingTask.FamilyMemberId,
                FamilyMemberName = existingTask.FamilyMember != null
                ? existingTask.FamilyMember.Name
                : null
            });
        });

        app.MapPost("/api/familytasks", async (FamilyPlannerDbContext db, CreateFamilyTaskDto newTask) =>
        {
            if (newTask.FamilyMemberId.HasValue)
            {
                bool memberExist = await db.FamilyMembers
                    .AnyAsync(member => member.Id == newTask.FamilyMemberId);
                if (!memberExist)
                {
                    return Results.BadRequest("FamilyMember does not exist");
                }
            }

            var task = new FamilyTask
            {
                Title = newTask.Title,
                Description = newTask.Description,
                FamilyMemberId = newTask.FamilyMemberId,
                IsCompleted = false
            };

            db.FamilyTasks.Add(task);

            await db.SaveChangesAsync();

            return Results.Created(
                $"/api/familytasks/{task.Id}",
                task
            );
        });

        app.MapPut("/api/familytasks/{id}", async (
            int id,
            FamilyPlannerDbContext db,
            UpdateFamilyTaskDto updatedTask) =>
        {
            // Find the existing task
            var existingTask = await db.FamilyTasks.FindAsync(id);

            // Return 404 if it doesn't exist
            if (existingTask == null)
            {
                return Results.NotFound();
            }

            // Validate FamilyMemberId if provided
            if (updatedTask.FamilyMemberId.HasValue)
            {
                bool memberExist = await db.FamilyMembers
                    .AnyAsync(member => member.Id == updatedTask.FamilyMemberId);
                if (!memberExist)
                {
                    return Results.BadRequest("FamilyMember does not exist");
                }
            }

            // Update the task properties
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            existingTask.FamilyMemberId = updatedTask.FamilyMemberId;

            // Save changes
            await db.SaveChangesAsync();

            // Return 204 No Content
            return Results.NoContent();
        });

        app.MapDelete("/api/familytasks/{id}", async (int id, FamilyPlannerDbContext db) =>
        {
            // Find the task
            var existingTask = await db.FamilyTasks.FindAsync(id);
            // Return 404 if it doesn't exist
            if (existingTask == null)
            {
                return Results.NotFound();
            }

            // Remove the task
            db.FamilyTasks.Remove(existingTask);

            // Save changes
            await db.SaveChangesAsync();

            // Return 204 No Content
            return Results.NoContent();
        });
    }
}