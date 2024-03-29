using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Web;
using BuildingBlocks.Dapr;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using JCR.Services.Observations.Api.ApplicationBuilderExtensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Observation API Service").Centered().Color(Color.DarkOrange));

// Create Application -https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((env, c) =>
{
    if (env.HostingEnvironment.IsDevelopment() || env.HostingEnvironment.IsEnvironment("tests") ||
        env.HostingEnvironment.IsStaging())
        c.ValidateScopes = true;
});

builder.Services.AddControllers(options =>
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

// Ignores if model state of controller is invalid - allows to return custom error response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddValidatedOptions<AppOptions>();

// register endpoints
builder.AddMinimalEndpoints();

/*----------------- Module Services Setup ------------------*/
builder.AddModulesServices();

var app = builder.Build();
// app.UseCloudEvents(); // Dapr cloud events middleware
/*----------------- Module Middleware Setup ------------------*/
await app.ConfigureModules();

app.UseRouting();
app.UseAppCors();

/*----------------- Module Routes Setup ------------------*/
app.MapModulesEndpoints();

// automatic discover minimal endpoints
app.MapMinimalEndpoints();

// https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0014
app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("docker"))
    app.UseCustomSwagger(); // swagger middleware should register last to discover all endpoints and its versions correctly

app.UseDapr();

await app.RunAsync();

public partial class Program
{
}
