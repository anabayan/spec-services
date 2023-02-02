using Asp.Versioning.Builder;
using BuildingBlocks.Abstractions.Web.Module;
using JCR.Services.AppliedAIService.Shared;

namespace JCR.Services.AppliedAIService.Extract;

internal class ExtractModuleConfiguration : IModuleConfiguration
{
    public const string Tag = "extract";
    public const string Prefix = $"{SharedModulesConfiguration.Prefix}/{Tag}";
    public static ApiVersionSet VersionSet { get; private set; } = default!;

    public WebApplicationBuilder AddModuleServices(WebApplicationBuilder builder)
    {
        return builder;
    }

    public Task<WebApplication> ConfigureModule(WebApplication app)
    {
        return Task.FromResult(app);
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        VersionSet = endpoints.NewApiVersionSet(Tag).Build();

        return endpoints;
    }
}
