using Ardalis.GuardClauses;
using AutoMapper;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Web.MinimalApi;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;

namespace JCR.Services.Observations.Create.Features.UploadingObservation;

public class UploadObservationEndpoint : ICommandMinimalEndpoint<UploadObservationRequest>
{
    private const string Name = "upload";
    private const string Description = "Upload observation document";
    public string GroupName => CreateModuleConfiguration.Tag;
    public string PrefixRoute => CreateModuleConfiguration.Prefix;
    public double Version => 1.0;

    public RouteHandlerBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        return builder.MapPost("/" + Name, HandleAsync)
            .Accepts<UploadObservationRequest>("multipart/form-data")
            .Produces<UploadObservationResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new SwaggerOperationAttribute(Description, Description))
            .WithName(Name)
            .WithDisplayName(Description);
    }


    public async Task<IResult> HandleAsync(
        HttpContext context,
        UploadObservationRequest request,
        ICommandProcessor commandProcessor,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        using (LogContext.PushProperty("Endpoint", nameof(UploadObservationEndpoint)))
        using (LogContext.PushProperty("SiteId", request.SiteId))
        using (LogContext.PushProperty("ProgramId", request.ProgramId))
        using (LogContext.PushProperty("TracerId", request.TracerId))
        {
            var command = mapper.Map<UploadObservationCommand>(request);
            var response = await commandProcessor.SendAsync(command, cancellationToken);

            return Results.Accepted("Observation Form Has Been Accepted For Processing", response);
        }
    }
}
