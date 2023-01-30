using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using JCR.Services.ImageProcessor.Extract;
using JCR.Services.ImageProcessor.Extract.Features.ExtractingObservation;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;

public class ExtractObservationEndpoint : ICommandMinimalEndpoint<ExtractObservationRequest>
{
    public string GroupName => ExtractModuleConfiguration.Tag;
    public string PrefixRoute => ExtractModuleConfiguration.Prefix;
    public double Version => 1.0;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder.MapPost("/observation", HandleAsync)
            // TODO: Add authorization
            // .RequireAuthorization()
            .Accepts<ExtractObservationRequest>("multipart/form-data")
            .Produces<ExtractObservationResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new SwaggerOperationAttribute(
                    "Extract observation from document",
                    "Extract observation from document"))
            .WithName("observation")
            .WithDisplayName("Extract observation from document.");
    }

    public async Task<IResult> HandleAsync(
        HttpContext context,
        ExtractObservationRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        using (LogContext.PushProperty("Endpoint", nameof(ExtractObservationEndpoint)))
        using (LogContext.PushProperty("SiteId", request.SiteId))
        {
            var command = mapper.Map<ExtractObservationCommand>(request);
            var response = await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Created("Observation Form Saved for Extraction", response);
        }
    }
}
