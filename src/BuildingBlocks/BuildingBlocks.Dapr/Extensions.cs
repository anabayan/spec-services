using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Dapr;

public static class Extensions
{
    // Adds Dapr Services to the DI container
    public static IServiceCollection AddDapr(
        this IServiceCollection services,
        params Type[] daprServices)
    {
        services.AddDaprClient();

        foreach (var daprService in daprServices) services.AddTransient(daprService);

        return services;
    }

    // Adds Dapr Middleware to the pipeline
    public static WebApplication UseDapr(this WebApplication app)
    {
        app.UseCloudEvents();

        return app;
    }
}
