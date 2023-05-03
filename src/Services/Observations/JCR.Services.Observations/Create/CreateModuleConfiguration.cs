using Asp.Versioning.Builder;
using BuildingBlocks.Abstractions.Web.Module;
using JCR.Services.Observations.Shared;

namespace JCR.Services.Observations.Create;

internal class CreateModuleConfiguration : IModuleConfiguration
{
    public const string Tag = "create";
    public const string Prefix = $"{SharedModulesConfiguration.Prefix}/{Tag}";
    public static ApiVersionSet VersionSet { get; } = default!;

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
        var endpointGroup = endpoints
            .MapApiGroup(Tag)
            .WithTags(Tag);

        var noVersion = endpointGroup.MapGroup(Prefix);

        var v1Group = endpointGroup
            .MapGroup(Prefix)
            .HasApiVersion(1.0);

        // endpointGroup.MapProcessObservationFormEndpoint();

        endpoints.MapSubscribeHandler();

        return endpoints;
    }
}
