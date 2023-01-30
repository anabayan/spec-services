using System.Reflection;
using BuildingBlocks.Abstractions.Web.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web.Extensions;

/// <summary>
///     The AddModulesServices method extends the WebApplicationBuilder type
///     with a method that takes an array of Assembly objects as a parameter. It then does the following:
///     1. If the scanAssemblies array is empty, it gets all of the assemblies that are currently
///     loaded in the AppDomain. Otherwise, it uses the provided assemblies.
///     2. It uses LINQ to select all of the types in the assemblies that meet the following criteria:
///     a. The type is a class
///     b. The type is not abstract
///     c. The type is not a generic type
///     d. The type is not an interface
///     e. The type has a public parameterless constructor
///     f. The type is either assignable to the IModuleConfiguration interface or the ISharedModulesConfiguration
///     interface.
///     3. It then calls the AddModulesDependencyInjection method for each of the types that were selected in step 2.
/// </summary>
public static class ModuleExtensions
{
    public static WebApplicationBuilder AddModulesServices(
        this WebApplicationBuilder webApplicationBuilder,
        params Assembly[] scanAssemblies)
    {
        var assemblies = scanAssemblies.Length > 0 ? scanAssemblies : AppDomain.CurrentDomain.GetAssemblies();

        var modulesConfiguration = assemblies.SelectMany(x => x.GetTypes()).Where(t =>
            t is { IsClass: true, IsAbstract: false, IsGenericType: false, IsInterface: false }
            && t.GetConstructor(Type.EmptyTypes) != null
            && typeof(IModuleConfiguration).IsAssignableFrom(t)).ToList();

        var sharedModulesConfiguration = assemblies.SelectMany(x => x.GetTypes()).Where(t =>
            t is { IsClass: true, IsAbstract: false, IsGenericType: false, IsInterface: false }
            && t.GetConstructor(Type.EmptyTypes) != null
            && typeof(ISharedModulesConfiguration).IsAssignableFrom(t)).ToList();

        foreach (var sharedModule in sharedModulesConfiguration)
            AddModulesDependencyInjection(webApplicationBuilder, sharedModule);

        foreach (var module in modulesConfiguration) AddModulesDependencyInjection(webApplicationBuilder, module);

        return webApplicationBuilder;
    }

    /// <summary>
    ///     <para>
    ///         The AddModulesDependencyInjection method takes a WebApplicationBuilder object and a Type object as parameters.
    ///         It does the following:
    ///     </para>
    ///     <para>
    ///         1. If the module type is assignable to the IModuleConfiguration interface,
    ///         it creates an instance of the type using the Activator.CreateInstance method,
    ///         adds the services for the module to the WebApplicationBuilder object's Services property,
    ///         and adds the instance to the Services property as a singleton.
    ///     </para>
    ///     <para>
    ///         2. If the module type is assignable to the ISharedModulesConfiguration interface,
    ///         it does the same thing as in step 1, but with the ISharedModulesConfiguration
    ///         interface and its corresponding method.
    ///     </para>
    /// </summary>
    private static void AddModulesDependencyInjection(
        WebApplicationBuilder webApplicationBuilder,
        Type module)
    {
        if (module.IsAssignableTo(typeof(IModuleConfiguration)))
        {
            var instantiatedType = (IModuleConfiguration)Activator.CreateInstance(module)!;
            instantiatedType.AddModuleServices(webApplicationBuilder);
            webApplicationBuilder.Services.AddSingleton(instantiatedType);
        }

        if (module.IsAssignableTo(typeof(ISharedModulesConfiguration)))
        {
            var instantiatedType = (ISharedModulesConfiguration)Activator.CreateInstance(module)!;
            instantiatedType.AddSharedModuleServices(webApplicationBuilder);
            webApplicationBuilder.Services.AddSingleton(instantiatedType);
        }
    }

    /// <summary>
    ///     <para>
    ///         The ConfigureModules method is an extension method for the WebApplication type that asynchronously configures
    ///         all of the modules that have been added to the application. It does the following:
    ///     </para>
    ///     <para>
    ///         1. It gets all of the IModuleConfiguration and ISharedModulesConfiguration instances that have been
    ///         registered in the application's Services property.
    ///     </para>
    ///     <para>
    ///         2. It calls the ConfigureSharedModule method on each of the ISharedModulesConfiguration instances, passing in
    ///         the WebApplication object.
    ///     </para>
    ///     <para>
    ///         3. It calls the ConfigureModule method on each of the IModuleConfiguration instances, passing in the
    ///         WebApplication object.
    ///     </para>
    ///     <para>4. It returns the WebApplication object.</para>
    /// </summary>
    public static async Task<WebApplication> ConfigureModules(this WebApplication app)
    {
        var moduleConfigurations = app.Services.GetServices<IModuleConfiguration>();
        var sharedModulesConfigurations = app.Services.GetServices<ISharedModulesConfiguration>();

        foreach (var sharedModule in sharedModulesConfigurations) await sharedModule.ConfigureSharedModule(app);

        foreach (var module in moduleConfigurations) await module.ConfigureModule(app);

        return app;
    }

    /// <summary>
    ///     <para>
    ///         The MapModulesEndpoints method is an extension method for the IEndpointRouteBuilder type that maps the
    ///         endpoints for all of the modules that have been added to the application. It does the following:
    ///     </para>
    ///     <para>
    ///         1. It gets all of the IModuleConfiguration and ISharedModulesConfiguration instances that have been
    ///         registered in the application's Services property.
    ///     </para>
    ///     <para>
    ///         2. It calls the MapSharedModuleEndpoints method on each of the ISharedModulesConfiguration instances, passing
    ///         in the IEndpointRouteBuilder object.
    ///     </para>
    ///     <para>
    ///         3. It calls the MapEndpoints method on each of the IModuleConfiguration instances, passing in the
    ///         IEndpointRouteBuilder object.
    ///     </para>
    ///     <para>4. It returns the IEndpointRouteBuilder object.</para>
    /// </summary>
    public static IEndpointRouteBuilder MapModulesEndpoints(this IEndpointRouteBuilder builder)
    {
        var modules = builder.ServiceProvider.GetServices<IModuleConfiguration>();
        var sharedModules = builder.ServiceProvider.GetServices<ISharedModulesConfiguration>();

        foreach (var module in sharedModules) module.MapSharedModuleEndpoints(builder);

        foreach (var module in modules) module.MapEndpoints(builder);

        return builder;
    }
}
