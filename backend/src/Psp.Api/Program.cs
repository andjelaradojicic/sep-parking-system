using DotNetEnv.Configuration;
using Microsoft.EntityFrameworkCore;
using Psp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv(
    Path.Combine(builder.Environment.ContentRootPath, ".env"),
    DotNetEnv.LoadOptions.NoEnvVars());

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PspDbContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention());

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>()!)
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await DbSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
