using FamilyPlanner.Api.Data;
using Microsoft.EntityFrameworkCore;
using FamilyPlanner.Api.Endpoints;
using FamilyPlanner.Api.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Register OpenAPI services
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();

// Register PostgreSQL database
builder.Services.AddDbContext<FamilyPlannerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services.AddScoped<FamilyTaskService>();
builder.Services.AddScoped<FamilyMemberService>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors("ReactFrontend");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapFamilyMemberEndpoints();
app.MapFamilyTaskEndpoints();

app.Run();


