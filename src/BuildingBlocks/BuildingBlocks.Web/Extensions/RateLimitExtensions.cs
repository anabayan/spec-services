using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Extensions;

public static class RateLimitExtensions
{
    public static WebApplicationBuilder AddCustomRateLimit(this WebApplicationBuilder builder)
    {
        // https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/
        // https://blog.maartenballiauw.be/post/2022/09/26/aspnet-core-rate-limiting-middleware.html
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // rate limiter that limits all to 100 requests per minute, per authenticated username (or hostname if not authenticated)
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        return builder;
    }

    public static WebApplication UseCustomRateLimit(this WebApplication app)
    {
        app.UseRateLimiter();
        return app;
    }
}
