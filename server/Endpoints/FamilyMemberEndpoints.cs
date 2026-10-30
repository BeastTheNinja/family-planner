using FamilyPlanner.Api.DTOs;
using FamilyPlanner.Api.Services;


namespace FamilyPlanner.Api.Endpoints;

public static class FamilyMemberEndpoints
{
    public static void MapFamilyMemberEndpoints(this WebApplication app)
    {

        app.MapGet("/api/familymembers", async (FamilyMemberService service) =>
        {
            var members = await service.GetAllAsync();
            var response = members.Select(member => new FamilyMemberResponseDto
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
            }).ToList();
            return Results.Ok(response);
        });

        app.MapGet("/api/familymembers/{id}", async (FamilyMemberService service, int id) =>
        {
            var member = await service.GetByIdAsync(id);

            var response = member != null ? new FamilyMemberResponseDto
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
            } : null;

            if (response == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyMember not found",
                    detail: $"The requested FamilyMember with ID {id} was not found."
                );
            }
            return Results.Ok(response);
        });

        app.MapPost("/api/familymembers", async (FamilyMemberService service, CreateFamilyMemberDto newMember) =>
        {
            var member = await service.CreateAsync(newMember);

            var response = new FamilyMemberResponseDto
            {
                Id = member.Id,
                Name = member.Name,
                IsActive = member.IsActive,
                Tasks = new List<FamilyMemberTaskDto>()
            };

            return Results.Created(
                $"/api/familymembers/{member.Id}",
                value: response
            );
        });

        app.MapPut("/api/familymembers/{id}", async (FamilyMemberService service, int id, UpdateFamilyMemberDto updatedFamilyMember) =>
        {
            var existingFamilyMember = await service.GetByIdAsync(id);
            if (existingFamilyMember == null)
            {
                return Results.Problem(
                     statusCode: 404,
                    title: "FamilyMember not found",
                    detail: "The requested FamilyMember was not found."
                );
            }

            await service.UpdateAsync(existingFamilyMember, updatedFamilyMember);

            var response = new FamilyMemberResponseDto
            {
                Id = existingFamilyMember.Id,
                Name = existingFamilyMember.Name,
                IsActive = existingFamilyMember.IsActive,
                Tasks = existingFamilyMember.Tasks.Select(task => new FamilyMemberTaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    IsCompleted = task.IsCompleted
                }).ToList()
            };

            return Results.Ok(response);
        });

        app.MapDelete("/api/familymembers/{id}", async (FamilyMemberService service, int id) =>
        {
            var deleteFamilyMember = await service.GetByIdAsync(id);
            if (deleteFamilyMember == null)
            {
                return Results.Problem(
                    statusCode: 404,
                    title: "FamilyMember not found",
                    detail: $"The requested FamilyMember with ID {id} was not found."
                );
            }
            if (deleteFamilyMember.Tasks.Any())
            {
                return Results.Problem(
                    statusCode: 409,
                    title: "Cannot delete FamilyMember with associated tasks",
                    detail: $"The requested FamilyMember with ID {id} has associated tasks and cannot be deleted."
                );
            }

            await service.DeleteAsync(deleteFamilyMember);

            return Results.NoContent();

        });
    }
}