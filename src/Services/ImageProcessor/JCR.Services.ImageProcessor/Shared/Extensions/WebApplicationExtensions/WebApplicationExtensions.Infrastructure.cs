using BuildingBlocks.Core.Extensions;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Logging;
using BuildingBlocks.Web.Extensions;
using Hellang.Middleware.ProblemDetails;
using Serilog;

namespace JCR.Services.ImageProcessor.Shared.Extensions.WebApplicationExtensions;

public static class WebApplicationExtensions
{
    public static async Task UseInfrastructure(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;

            // https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-excluding-health-check-endpoints-from-serilog-request-logging/#customising-the-log-level-used-for-serilog-request-logs
            options.GetLevel = LogEnricher.GetLogLevel;
        });

        // orders for middlewares is important and problemDetails middleware should be placed on top
        app.UseProblemDetails();

        app.UseRequestLogContextMiddleware();

        // TODO Add Authentication and Authorization here
        // app.UseAuthentication();
        // app.UseAuthorization();

        app.UseCustomRateLimit();

        if (app.Environment.IsTest() == false)
            app.UseCustomHealthCheck();

        // Configure the prometheus endpoint for scraping metrics
        // NOTE: This should only be exposed on an internal port!
        // .RequireHost("*:9100");
        app.MapPrometheusScrapingEndpoint();
    }
}
