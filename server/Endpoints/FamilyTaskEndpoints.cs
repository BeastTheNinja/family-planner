using FamilyPlanner.Api.Models;
using FamilyPlanner.Api.Data;
using FamilyPlanner.Api.DTOs;
using FamilyPlanner.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FamilyPlanner.Api.Endpoints;

public static class FamilyTaskEndpoints
{
    public static void MapFamilyTaskEndpoints(this WebApplication app)
    {
        app.MapGet("/api/familytasks", async (FamilyTaskService service) =>
        {
            var tasks = (await service.GetAllAsync())
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
                .ToList();
            return Results.Ok(tasks);
        });

        app.MapGet("/api/familytasks/{id}", async (int id, FamilyPlannerDbContext db) =>
        {

            var existingTask = await db.FamilyTasks
                .Include(task => task.FamilyMember)
                .FirstOrDefaultAsync(task => task.Id == id);


            if (existingTask == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyTask not found",
                    detail: $"The requested FamilyTask with ID {id} was not found."
                );
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
                    return Results.Problem(
                        statusCode: 400,
                        title: "Invalid FamilyMemberId",
                        detail: $"The provided FamilyMemberId {newTask.FamilyMemberId} does not exist."
                    );
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

            var memberName = await db.FamilyMembers
            .Where(member => member.Id == task.FamilyMemberId)
            .Select(member => member.Name)
            .FirstOrDefaultAsync();

            var response = new FamilyTaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                FamilyMemberId = task.FamilyMemberId,
                FamilyMemberName = memberName
            };

            return Results.Created(
                $"/api/familytasks/{task.Id}",
                response
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
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyTask not found",
                    detail: $"The requested FamilyTask with ID {id} was not found."
                );
            }

            // Validate FamilyMemberId if provided
            if (updatedTask.FamilyMemberId.HasValue)
            {
                bool memberExist = await db.FamilyMembers
                    .AnyAsync(member => member.Id == updatedTask.FamilyMemberId);
                if (!memberExist)
                {
                    return Results.Problem(
                        statusCode: 400,
                        title: "Invalid FamilyMemberId",
                        detail: $"The provided FamilyMemberId {updatedTask.FamilyMemberId} does not exist."
                    );
                }
            }

            // Update the task properties
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            existingTask.FamilyMemberId = updatedTask.FamilyMemberId;

            // Save changes
            await db.SaveChangesAsync();

            var memberName = await db.FamilyMembers
            .Where(member => member.Id == existingTask.FamilyMemberId)
            .Select(member => member.Name)
            .FirstOrDefaultAsync();

            var response = new FamilyTaskResponseDto
            {
                Id = existingTask.Id,
                Title = existingTask.Title,
                Description = existingTask.Description,
                IsCompleted = existingTask.IsCompleted,
                FamilyMemberId = existingTask.FamilyMemberId,
                FamilyMemberName = memberName
            };

            // Return 200 OK with the updated task
            return Results.Ok(response);
        });

        app.MapDelete("/api/familytasks/{id}", async (int id, FamilyPlannerDbContext db) =>
        {
            // Find the task
            var existingTask = await db.FamilyTasks.FindAsync(id);
            // Return 404 if it doesn't exist
            if (existingTask == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyTask not found",
                    detail: $"The requested FamilyTask with ID {id} was not found."
                );
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