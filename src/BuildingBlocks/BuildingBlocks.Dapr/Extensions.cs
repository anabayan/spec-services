using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Dapr;

public static class Extensions
{
    // Adds Dapr Services to the DI container
    public static WebApplicationBuilder AddDapr(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddDaprClient();

        return builder;
    }

    // Adds Dapr Middleware to the pipeline
    public static WebApplication UseDapr(this WebApplication app)
    {
        app.UseCloudEvents();

        return app;
    }
}
