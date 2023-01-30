using Asp.Versioning;
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Web.Extensions;

public static class VersioningExtensions
{
    public static WebApplicationBuilder AddCustomVersioning(
        this WebApplicationBuilder builder,
        Action<ApiVersioningOptions>? configurator = null)
    {
        // Support versioning in minimal apis with (Asp.Versioning.Http) dll
        builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new QueryStringApiVersionReader(),
                    new UrlSegmentApiVersionReader());

                configurator?.Invoke(options);
            })
            .AddApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    options.SubstituteApiVersionInUrl = true;
                })
            .AddMvc(); // https://www.nuget.org/packages/Asp.Versioning.Mvc.ApiExplorer

        return builder;
    }
}
