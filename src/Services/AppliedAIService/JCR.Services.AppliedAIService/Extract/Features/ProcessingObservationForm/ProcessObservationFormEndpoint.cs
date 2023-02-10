// using AutoMapper;
// using BuildingBlocks.Abstractions.CQRS.Commands;
// using BuildingBlocks.Abstractions.Web.MinimalApi;
// using Dapr.Client;
//
// namespace JCR.Services.AppliedAIService.Extract.Features.ProcessingObservationForm;
//
// public class ProcessObservationFormEndpoint : ICommandMinimalEndpoint<string>
// {
//     private readonly DaprClient _dapr;
//
//     public ProcessObservationFormEndpoint(DaprClient dapr)
//     {
//         _dapr = dapr;
//     }
//
//     public string GroupName => ExtractModuleConfiguration.Tag;
//     public string PrefixRoute => ExtractModuleConfiguration.Prefix;
//     public double Version => 1.0;
//
//     public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
//     {
//         return builder.MapPost("/processform", HandleAsync)
//             .WithTopic(
//                 "jcr-services-bus",
//                 "ObservationFormUploaded")
//             // TODO: Add authorization
//             // .RequireAuthorization()
//             .Produces<ProcessObservationFormResponse>(StatusCodes.Status201Created)
//             .Produces(StatusCodes.Status401Unauthorized)
//             .Produces(StatusCodes.Status400BadRequest)
//             .ExcludeFromDescription();
//     }
//
//     public async Task<IResult> HandleAsync(
//         HttpContext context,
//         [FromBody] string FileName,
//         ICommandProcessor commandProcessor,
//         IMapper mapper,
//         CancellationToken cancellationToken)
//     {
//         Console.WriteLine("SUBBED TO TOPIC", FileName);
//         await _dapr.PublishEventAsync("jcr-services-bus", "observationformprocessed", "Successful event subbed");
//         return Results.Created("Just nothing", "observation");
//     }
// }

using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using Serilog.Context;

namespace JCR.Services.AppliedAIService.Extract.Features.ProcessingObservationForm;

/// <summary>
///     There is an open issue with the Dapr .NET SDK that prevents the use of versioning and topic subscriptions
///     So, for now, we are not auto registeting with IMinimalEndpoint interface.
///     Instead we are registering by hand at the top level and excluding from the description.
///     https://github.com/dapr/dotnet-sdk/issues/791
///     https://github.com/dapr/dotnet-sdk/issues/882
/// </summary>
public static class ProcessObservationFormEndpoint
{
    internal static RouteHandlerBuilder MapProcessObservationFormEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/processform", HandleAsync)
            .WithTopic(
                "jcr-services-bus",
                "ObservationFormUploaded")
            .ExcludeFromDescription();
    }

    private static async Task<IResult> HandleAsync(
        ObservationFormUploadedEvent @event,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty("Endpoint", nameof(ProcessObservationFormEndpoint)))
        {
            var command = mapper.Map<ProcessObservationFormCommand>(@event);
            var response = await commandProcessor.SendAsync(command, cancellationToken);
            return Results.Created("Observation Created", response);
        }
    }
}
