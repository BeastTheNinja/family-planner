using FamilyPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;
using FamilyPlanner.Api.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Register OpenAPI services
builder.Services.AddOpenApi();
builder.Services.AddValidation();

// Register PostgreSQL database
builder.Services.AddDbContext<FamilyPlannerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFamilyMemberEndpoints();
app.MapFamilyTaskEndpoints();

app.Run();


