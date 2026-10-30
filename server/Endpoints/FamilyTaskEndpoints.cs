using FamilyPlanner.Api.DTOs;
using FamilyPlanner.Api.Services;

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

        app.MapGet("/api/familytasks/{id}", async (int id, FamilyTaskService service) =>
        {

            var existingTask = await service.GetByIdAsync(id);

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

        app.MapPost("/api/familytasks", async (
            CreateFamilyTaskDto newTask,
            FamilyTaskService service) =>
        {
            if (newTask.FamilyMemberId.HasValue)
            {
                bool memberExist = await service.FamilyMemberExistsAsync(
                    newTask.FamilyMemberId.Value);

                if (!memberExist)
                {
                    return Results.Problem(
                        statusCode: 400,
                        title: "Invalid FamilyMemberId",
                        detail: $"The provided FamilyMemberId {newTask.FamilyMemberId} does not exist."
                    );
                }
            }

            var task = await service.CreateAsync(newTask);

            var memberName = await service.GetFamilyMemberNameAsync(task.FamilyMemberId);

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
            UpdateFamilyTaskDto updatedTask,
            FamilyTaskService service) =>
        {
            // Find the existing task
            var existingTask = await service.GetByIdAsync(id);

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
                bool memberExist = await service.FamilyMemberExistsAsync(
                    updatedTask.FamilyMemberId.Value);
                if (!memberExist)
                {
                    return Results.Problem(
                        statusCode: 400,
                        title: "Invalid FamilyMemberId",
                        detail: $"The provided FamilyMemberId {updatedTask.FamilyMemberId} does not exist."
                    );
                }
            }

            var task = await service.UpdateAsync(existingTask, updatedTask);

            var memberName = await service.GetFamilyMemberNameAsync(task.FamilyMemberId);

            var response = new FamilyTaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                FamilyMemberId = task.FamilyMemberId,
                FamilyMemberName = memberName
            };

            // Return 200 OK with the updated task
            return Results.Ok(response);
        });

        app.MapDelete("/api/familytasks/{id}", async (int id, FamilyTaskService service) =>
        {
            // Find the task
            var existingTask = await service.GetByIdAsync(id);
            // Return 404 if it doesn't exist
            if (existingTask == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyTask not found",
                    detail: $"The requested FamilyTask with ID {id} was not found."
                );
            }

            await service.DeleteAsync(existingTask);

            // Return 204 No Content
            return Results.NoContent();
        });
    }
}