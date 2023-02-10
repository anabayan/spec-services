using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Core.IdsGenerator;
using Dapr.Client;
using FluentValidation;

namespace JCR.Services.AppliedAIService.Extract.Features.ExtractingObservation;

public record ExtractObservationCommand(
    IFormFile File,
    int SiteId,
    int ProgramId,
    int TracerId
) : ICommand<ExtractObservationResponse>
{
    public long Id { get; init; } = SnowFlakeIdGenerator.NewId();
}

public class ExtractObservationValidator : AbstractValidator<ExtractObservationCommand>
{
    public ExtractObservationValidator()
    {
        // TODO: Add validators (Create multiple file validators?) for file -> filename, extension, filename endings, etc.
        // https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool-input-validation#controls-users
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.SiteId).NotEmpty().GreaterThan(0).WithMessage("Site is required.");
        RuleFor(x => x.ProgramId).NotEmpty().GreaterThan(0).WithMessage("Program is required.");
        RuleFor(x => x.TracerId).NotEmpty().GreaterThan(0).WithMessage("Tracer is required.");
    }
}

public class ExtractObservationHandler : ICommandHandler<ExtractObservationCommand, ExtractObservationResponse>
{
    private readonly IBlobUpload _blobUpload;
    private readonly DaprClient _dapr;

    public ExtractObservationHandler(IBlobUpload blobUpload, DaprClient dapr)
    {
        _blobUpload = blobUpload;
        _dapr = dapr;
    }

    public async Task<ExtractObservationResponse> Handle(
        ExtractObservationCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        await _blobUpload.UploadAsync(request.File, cancellationToken);

        await _dapr.PublishEventAsync(
            "jcr-services-bus",
            "ObservationFormUploaded",
            new ObservationFormUploadedEvent(
                request.File.FileName.Replace(" ", "_", StringComparison.OrdinalIgnoreCase),
                request.SiteId,
                request.ProgramId,
                request.TracerId),
            cancellationToken);

        return await Task.FromResult(new ExtractObservationResponse(true));
    }
}
