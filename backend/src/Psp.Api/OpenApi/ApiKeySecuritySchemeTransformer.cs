using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Psp.Api.OpenApi;

public sealed class ApiKeySecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private const string MerchantIdScheme = "MerchantId";
    private const string ApiKeyScheme = "ApiKey";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken ct)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[MerchantIdScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Merchant-Id",
        };
        document.Components.SecuritySchemes[ApiKeyScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Api-Key",
        };

        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(MerchantIdScheme, document)] = [],
            [new OpenApiSecuritySchemeReference(ApiKeyScheme, document)] = [],
        };
        foreach (var path in document.Paths.Where(p => p.Key.StartsWith("/api/transactions")))
            foreach (var operation in path.Value.Operations!.Values)
                (operation.Security ??= []).Add(requirement);

        return Task.CompletedTask;
    }
}