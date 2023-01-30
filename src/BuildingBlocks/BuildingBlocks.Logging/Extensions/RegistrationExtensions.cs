using BuildingBlocks.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace BuildingBlocks.Logging;

public static class RegistrationExtensions
{
    public static WebApplicationBuilder AddCustomSerilog(
        this WebApplicationBuilder builder,
        string sectionName = "Serilog",
        Action<LoggerConfiguration>? extraConfigure = null)
    {
        var serilogOptions = builder.Configuration.BindOptions<SerilogOptions>(sectionName);

        // Serilog replace `ILoggerFactory`,It replaces microsoft `LoggerFactory` class with `SerilogLoggerFactory`,
        // so `ConsoleLoggerProvider` and other default microsoft logger providers don't instantiate at all with serilog
        builder.Host.UseSerilog((context, serviceProvider, loggerConfiguration) =>
        {
            extraConfigure?.Invoke(loggerConfiguration);

            loggerConfiguration
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)

                // .Enrich.WithSpan()
                // .Enrich.WithBaggage()
                .Enrich.WithCorrelationIdHeader()
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails();

            loggerConfiguration.ReadFrom.Configuration(context.Configuration, sectionName);

            if (serilogOptions.UseConsole)
            {
                if (serilogOptions.UseElasticsearchJsonFormatter)
                    loggerConfiguration.WriteTo.Async(writeTo =>
                        writeTo.Console(new ExceptionAsObjectJsonFormatter(renderMessage: true)));
                else
                    loggerConfiguration.WriteTo.Async(
                        writeTo => writeTo.Console(outputTemplate: serilogOptions.LogTemplate));
            }

            if (!string.IsNullOrEmpty(serilogOptions.ElasticSearchUrl))
                loggerConfiguration.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(serilogOptions.ElasticSearchUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        IndexFormat = $"{builder.Environment.ApplicationName}-{DateTime.UtcNow:yyyy-MM}"
                    });

            if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
                loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);

            if (!string.IsNullOrEmpty(serilogOptions.LogPath))
                loggerConfiguration.WriteTo.Async(writeTo =>
                    writeTo.File(
                        serilogOptions.LogPath,
                        outputTemplate: serilogOptions.LogTemplate,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true));
        });

        return builder;
    }
}
