using System.Reflection;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.Extensions.ServiceCollection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Dapr;

public static class Extensions
{
    // Adds Dapr Services to the DI container
    public static IServiceCollection AddDaprServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly[]? assemblies = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        params Type[] daprServices)
    {
        services.AddDaprClient();
        services.AddValidatedOptions<DaprOptions>();
        configuration.BindOptions<DaprOptions>();

        services.AddDaprServices(
            assemblies,
            serviceLifetime,
            daprServices);

        return services;
    }

    private static void AddDaprServices(
        this IServiceCollection serviceCollection,
        Assembly[]? assemblies = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        params Type[] daprServices)
    {
        assemblies ??= AppDomain.CurrentDomain.GetAssemblies();
        var implementations = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Intersect(daprServices).Any());

        foreach (var implementation in implementations)
        {
            var interfaceType = implementation.GetInterfaces().Intersect(daprServices).First();
            serviceCollection.Add(new ServiceDescriptor(interfaceType, implementation, serviceLifetime));
        }
    }

    // Adds Dapr Middleware to the pipeline
    public static WebApplication UseDapr(this WebApplication app)
    {
        app.UseCloudEvents();

        return app;
    }
}
