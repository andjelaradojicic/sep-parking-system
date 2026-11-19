using DotNetEnv.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Psp.Api.Auth;
using Psp.Api.Data;
using Psp.Api.OpenApi;
using Psp.Api.Services;
using Scalar.AspNetCore;
using Sep.Shared.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddDotNetEnv(
    Path.Combine(builder.Environment.ContentRootPath, ".env"),
    DotNetEnv.LoadOptions.NoEnvVars());

// Add services to the container.

builder.Services.AddControllers(o =>
    o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()));
builder.Services.AddSepProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(o => o.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>());

builder.Services.AddDbContext<PspDbContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention());

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>()!)
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, null);

builder.Services.AddScoped<TransactionService>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();

    using (var scope = app.Services.CreateScope())
    {
        await scope.ServiceProvider.GetRequiredService<PspDbContext>().Database.MigrateAsync();
    }

    await DbSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
