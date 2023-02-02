using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Caching;
using BuildingBlocks.Caching.Behaviours;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.IdsGenerator;
using BuildingBlocks.Core.Registrations;
using BuildingBlocks.Dapr;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Logging;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.Swagger;
using BuildingBlocks.Validation;
using BuildingBlocks.Web.Extensions;
using DotNetEnv;
using JCR.Services.ImageProcessor.Extract;

namespace JCR.Services.ImageProcessor.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        SnowFlakIdGenerator.Configure(1);

        builder.Services.AddCore(builder.Configuration);

        // TODO: Authentication, Authorization

        // TODO: Email

        builder.Services.AddCqrs(pipelines: new[]
        {
            typeof(RequestValidationBehavior<,>), typeof(StreamRequestValidationBehavior<,>),
            typeof(StreamLoggingBehavior<,>), typeof(LoggingBehavior<,>), typeof(StreamCachingBehavior<,>),
            typeof(CachingBehavior<,>), typeof(InvalidateCachingBehavior<,>)
        });

        // https://github.com/tonerdo/dotnet-env
        Env.TraversePath().Load();

        builder.Configuration.AddEnvironmentVariables("jcr_imageprocessor_env_");

        builder.AddCustomVersioning();

        builder.AddCustomSwagger(typeof(ImageProcessorRoot).Assembly);

        builder.Services.AddHttpContextAccessor();

        //TODO: Add from Dapr Here (Messagebus, bindings, stores etc);

        builder.AddCompression();
        builder.AddCustomProblemDetails();

        builder.AddCustomSerilog();

        //TODO: Add OpenTelemetry Here custom or Dapr
        builder.AddCustomOpenTelemetry();

        // Add required services for the current service in an array
        builder.Services.AddDapr(typeof(IBlobUpload));

        // https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
        builder.AddCustomRateLimit();

        if (!builder.Environment.IsTest())
            builder.Services.AddCustomHealthCheck(healthChecksBuilder =>
            {
                // Do dapr and other health checks here
            });

        builder.Services.AddCustomValidators(Assembly.GetExecutingAssembly());

        //Add Mappers Here
        builder.Services.AddAutoMapper(x =>
        {
            x.AddProfile<ExtractMappers>();
        });

        builder.AddCustomCaching();

        return builder;
    }
}
