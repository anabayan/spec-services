using System.Reflection;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Caching;
using BuildingBlocks.Core.Extensions;
using EasyCaching.Redis;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;

namespace BuildingBlocks.Caching;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomCaching(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies)
    {
        // https://www.twilio.com/blog/provide-default-configuration-to-dotnet-applications
        var cacheOptions = builder.Configuration.BindOptions<CacheOptions>();
        Guard.Against.Null(cacheOptions);

        var scanAssemblies = assemblies.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

        AddCachingRequests(builder.Services, scanAssemblies);

        builder.Services.AddEasyCaching(option =>
        {
            if (cacheOptions.RedisCacheOptions is not null)
                option.UseRedis(
                    config =>
                    {
                        config.DBConfig =
                            new RedisDBOptions { Configuration = cacheOptions.RedisCacheOptions.ConnectionString };
                        config.SerializerName = cacheOptions.SerializationType;
                    },
                    nameof(CacheProviderType.Redis));

            option.UseInMemory(
                config =>
                {
                    config.SerializerName = cacheOptions.SerializationType;
                },
                nameof(CacheProviderType.InMemory));

            switch (cacheOptions.SerializationType)
            {
                case nameof(CacheSerializationType.Json):
                    option.WithJson(
                        jsonSerializerSettingsConfigure:
                        x => x.TypeNameHandling = TypeNameHandling.None,
                        nameof(CacheSerializationType.Json));
                    break;
                case nameof(CacheSerializationType.MessagePack):
                    option.WithMessagePack(nameof(CacheSerializationType.MessagePack));
                    break;
            }
        });

        return builder;
    }

    private static IServiceCollection AddCachingRequests(
        this IServiceCollection services,
        params Assembly[] assembliesToScan)
    {
        // ICacheRequest discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan.Any() ? assembliesToScan : AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(
                classes => classes.AssignableTo(typeof(ICacheRequest<,>)),
                false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        // IInvalidateCacheRequest discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan.Any() ? assembliesToScan : AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(
                classes => classes.AssignableTo(typeof(IInvalidateCacheRequest<,>)),
                false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
