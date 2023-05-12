using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.IdsGenerator;
using FluentValidation;
using JCR.Services.Shared.Observations.Create.Events.v1;

namespace JCR.Services.Observations.Create.Features.UploadingObservation;

public record UploadObservationCommand(
    IFormFile File,
    int SiteId,
    int ProgramId,
    int TracerId
) : ICommand<UploadObservationResponse>
{
    public long Id { get; init; } = SnowFlakeIdGenerator.NewId();
}

public class UploadObservationValidator : AbstractValidator<UploadObservationCommand>
{
    public UploadObservationValidator()
    {
        // TODO: Add validators (Create multiple file validators?) for file -> filename, extension, filename endings, etc.
        // https://learn.microsoft.com/en-us/azure/security/develop/threat-modeling-tool-input-validation#controls-users
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.SiteId).NotEmpty().GreaterThan(0).WithMessage("Site is required.");
        RuleFor(x => x.ProgramId).NotEmpty().GreaterThan(0).WithMessage("Program is required.");
        RuleFor(x => x.TracerId).NotEmpty().GreaterThan(0).WithMessage("Tracer is required.");
    }
}

public class UploadObservationCommandHandler : ICommandHandler<UploadObservationCommand, UploadObservationResponse>
{
    private readonly IBlobUpload _blobUpload;
    private readonly ILogger<UploadObservationCommandHandler> _logger;
    private readonly IBus _messageBus;

    public UploadObservationCommandHandler(
        ILogger<UploadObservationCommandHandler> logger,
        IBlobUpload blobUpload,
        IBus messageBus)
    {
        _blobUpload = blobUpload;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task<UploadObservationResponse> Handle(
        UploadObservationCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        _logger.LogInformation("Uploading observation form {Request} to blob storage", request);

        await _blobUpload.UploadAsync(request.File, cancellationToken);

        await _messageBus.PublishEventAsync(
            "jcr-services-bus",
            "EProducts/Services/Observations/ObservationUploaded",
            new ObservationUploadedV1(
                request.File.FileName.Replace(" ", "_", StringComparison.OrdinalIgnoreCase),
                request.SiteId,
                request.ProgramId,
                request.TracerId),
            cancellationToken);

        return new UploadObservationResponse(true);
    }
}
