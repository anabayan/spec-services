using BuildingBlocks.Validation;
using Hellang.Middleware.ProblemDetails;
using Newtonsoft.Json;

namespace JCR.Services.AppliedAIService.Shared.Extensions.WebApplicationBuilderExtensions;

public static partial class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddCustomProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(x =>
        {
            x.ShouldLogUnhandledException = (httpContext, exception, problemDetails) =>
            {
                var env = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };

            // Control when an exception is included
            x.IncludeExceptionDetails = (ctx, _) =>
            {
                // Fetch services from HttpContext.RequestServices
                var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
                return env.IsDevelopment() || env.IsStaging();
            };

            // Create problem details for FluentValidation RequestValidationBehavior
            x.Map<ValidationException>(ex => new ProblemDetails
            {
                Title = "Validation Rules Broken",
                Status = StatusCodes.Status400BadRequest,
                Detail = JsonConvert.SerializeObject(ex.ValidationResultModel.Errors),
                Type = "https://jcrinc.com/input-validation-rules-error"
            });

            // Argument exception
            x.Map<ArgumentException>(ex => new ProblemDetails
            {
                Title = "argument is invalid",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message,
                Type = "https://jcrinc.com/argument-error"
            });
        });

        return builder;
    }
}
