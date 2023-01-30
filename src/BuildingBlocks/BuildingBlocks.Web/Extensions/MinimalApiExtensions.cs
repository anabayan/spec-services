using BuildingBlocks.Abstractions.Web.MinimalApi;
using LinqKit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scrutor;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
///     This  methods here are used to set up a system for grouping and mapping endpoints.
///     The code creates a new scope using the CreateScope method of the ServiceProvider property of the builder object.
///     It then gets a list of all of the IMinimalEndpoint instances that have been registered in the scope's
///     ServiceProvider.
///     The code then groups the IMinimalEndpoint instances by their GroupName property and creates a dictionary that maps
///     each group name to an
///     ApiGroup object created using the MapApiGroup method of the builder object and the group name as the tag.
///     Next, the code groups the IMinimalEndpoint instances by their GroupName, PrefixRoute, and Version properties and
///     creates a
///     dictionary that maps each group to a Group object created using the MapGroup method of the corresponding ApiGroup
///     object and the PrefixRoute and Version as arguments.
///     The code then groups the IMinimalEndpoint instances by their GroupName and Version properties and creates an
///     anonymous
///     type for each group that contains the Version, GroupName, and a list of IMinimalEndpoint instances.
///     Finally, the code iterates over the anonymous types and for each one, gets the corresponding Group object from the
///     dictionary
///     using the GroupName as the key. It then calls the MapEndpoint method on each IMinimalEndpoint instance,
///     passing in the Group object as an argument.
///     The method then returns the builder object.
/// </summary>
public static class MinimalApiExtensions
{
    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        applicationBuilder.Services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .As<IMinimalEndpoint>()
            .WithLifetime(lifetime));

        return applicationBuilder.Services;
    }

    public static IServiceCollection AddMinimalEndpoints(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        services.Scan(scan => scan
            .FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .As<IMinimalEndpoint>()
            .WithLifetime(lifetime));

        return services;
    }

    /// <summary>
    ///     Map registered minimal apis.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder builder)
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>().ToList();

        var versionGroups =
            endpoints.GroupBy(x => x.GroupName)
                .ToDictionary(x => x.Key, c => builder.MapApiGroup(c.Key).WithTags(c.Key));

        var versionSubGroups = endpoints.GroupBy(x => new { x.GroupName, x.PrefixRoute, x.Version })
            .ToDictionary(
                x => x.Key,
                c => versionGroups[c.Key.GroupName].MapGroup(c.Key.PrefixRoute).HasApiVersion(c.Key.Version));

        var endpointVersions = endpoints.GroupBy(x => new { x.GroupName, x.Version }).Select(x => new
        {
            Verion = x.Key.Version, x.Key.GroupName, Endpoints = x.Select(v => v)
        });

        foreach (var endpointVersion in endpointVersions)
        {
            var versionGroup = versionSubGroups
                .FirstOrDefault(x => x.Key.GroupName == endpointVersion.GroupName).Value;

            endpointVersion.Endpoints.ForEach(ep =>
            {
                ep.MapEndpoint(versionGroup);
            });
        }

        return builder;
    }
}
