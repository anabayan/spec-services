using BuildingBlocks.Abstractions.Web.Module;
using BuildingBlocks.Core;
using JCR.Services.Observations.Shared.Extensions.WebApplicationBuilderExtensions;
using JCR.Services.Observations.Shared.Extensions.WebApplicationExtensions;

namespace JCR.Services.Observations.Shared;

public class SharedModulesConfiguration : ISharedModulesConfiguration
{
    public const string Prefix = "api/v{version:apiVersion}/Observations";

    public WebApplicationBuilder AddSharedModuleServices(WebApplicationBuilder builder)
    {
        builder.AddInfrastructure();

        // TODO Add State Store(s) here
        // In other APIs this might mean adding storage and associated contexts(eg. ObservationReadDbContext)
        return builder;
    }

    public async Task<WebApplication> ConfigureSharedModule(WebApplication app)
    {
        await app.UseInfrastructure();

        ServiceActivator.Configure(app.Services); // app.Services is of type IServiceProvider

        return app;
    }

    public IEndpointRouteBuilder MapSharedModuleEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", (HttpContext context) =>
        {
            var requestId = context.Request.Headers.TryGetValue("X-Request-InternalCommandId", out var requestIdHeader)
                ? requestIdHeader.FirstOrDefault()
                : string.Empty;

            return $"Observation Service Apis, RequestId: {requestId}.";
        }).ExcludeFromDescription();

        return endpoints;
    }
}
