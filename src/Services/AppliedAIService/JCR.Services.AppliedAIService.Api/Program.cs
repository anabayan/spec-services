using BuildingBlocks.Core.Extensions.ServiceCollection;
using BuildingBlocks.Core.Web;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using BuildingBlocks.Web.Extensions;
using JCR.Services.ImageProcessor.Api.Extensions.ApplicationBuilderExtensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using Spectre.Console;

AnsiConsole.Write(new FigletText("ImageProcessor Service").Centered().Color(Color.DarkOrange));

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

//register endpoints
builder.AddMinimalEndpoints();

/*----------------- Module Services Setup ------------------*/
builder.AddModulesServices();

var app = builder.Build();

/*----------------- Module Middleware Setup ------------------*/
await app.ConfigureModules();

// https://thecodeblogger.com/2021/05/27/asp-net-core-web-application-routing-and-endpoint-internals/
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#routing-basics
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#endpoints
// https://stackoverflow.com/questions/57846127/what-are-the-differences-between-app-userouting-and-app-useendpoints
// in .net 6 and above we don't need UseRouting and UseEndpoints but if ordering is important we should write it
// app.UseRouting();
app.UseAppCors();

/*----------------- Module Routes Setup ------------------*/
app.MapModulesEndpoints();

// automatic discover minimal endpoints
app.MapMinimalEndpoints();

// https://learn.microsoft.com/en-us/aspnet/core/diagnostics/asp0014
app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("docker"))
    app.UseCustomSwagger(); // swagger middleware should register last to discover all endpoints and its versions correctly

await app.RunAsync();

public partial class Program
{
}
