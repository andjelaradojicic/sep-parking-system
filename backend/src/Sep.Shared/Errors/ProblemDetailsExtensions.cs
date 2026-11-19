using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Sep.Shared.Errors;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddSepProblemDetails(this IServiceCollection services)
    {
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
        {
            var problem = ctx.ProblemDetails;
            if (problem.Extensions.ContainsKey("code"))
                return;

            var code = problem.Status switch
            {
                StatusCodes.Status400BadRequest => "VALIDATION_FAILED",
                StatusCodes.Status401Unauthorized => "UNAUTHORIZED",
                StatusCodes.Status403Forbidden => "FORBIDDEN",
                StatusCodes.Status404NotFound => "NOT_FOUND",
                _ => "INTERNAL_ERROR",
            };
            problem.Extensions["code"] = code;
            problem.Type ??= TypeFor(code);
        });
        return services;
    }

    internal static string TypeFor(string code) =>
        $"https://sep.local/errors/{code.ToLowerInvariant().Replace('_', '-')}";
}

internal sealed class DomainExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception exception, CancellationToken ct)
    {
        if (exception is not DomainException ex)
            return false;

        ProblemDetails problem = ex.Errors is null
            ? new ProblemDetails()
            : new ValidationProblemDetails(ex.Errors);
        problem.Status = ex.StatusCode;
        problem.Title = ex.Message;
        problem.Type = ProblemDetailsExtensions.TypeFor(ex.Code);
        problem.Extensions["code"] = ex.Code;

        http.Response.StatusCode = ex.StatusCode;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            ProblemDetails = problem,
            Exception = exception,
        });
    }
}
