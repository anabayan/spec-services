using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Core.IdsGenerator;
using FluentValidation;
using JCR.Services.AppliedAIService.Extract.Services;
using JCR.Services.Shared.Observations.Create.Events.v1;

namespace JCR.Services.AppliedAIService.Extract.Features.ProcessingObservationForm;

public record ProcessObservationFormCommand
    (string FileName, int SiteId, int ProgramId, int TracerId) : ICommand<ProcessObservationFormResponse>
{
    public long Id { get; init; } = SnowFlakeIdGenerator.NewId();
}

public class ExtractObservationValidator : AbstractValidator<ProcessObservationFormCommand>
{
    public ExtractObservationValidator()
    {
        RuleFor(x => x.FileName).NotNull();
        RuleFor(x => x.SiteId).NotEmpty().GreaterThan(0).WithMessage("Site is required.");
        RuleFor(x => x.ProgramId).NotEmpty().GreaterThan(0).WithMessage("Program is required.");
        RuleFor(x => x.TracerId).NotEmpty().GreaterThan(0).WithMessage("Tracer is required.");
    }
}

public class ProcessObservationCommandHandler
    : ICommandHandler<ProcessObservationFormCommand, ProcessObservationFormResponse>
{
    private readonly FormRecognizerService _formRecognizerService;
    private readonly ILogger<ProcessObservationCommandHandler> _logger;
    private readonly IBus _messageBus;
    private readonly IStateStore _stateStore;

    public ProcessObservationCommandHandler(
        ILogger<ProcessObservationCommandHandler> logger,
        FormRecognizerService formRecognizerService,
        IStateStore stateStore, IBus messageBus)
    {
        _formRecognizerService = formRecognizerService;
        _stateStore = stateStore;
        _messageBus = messageBus;
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    public async Task<ProcessObservationFormResponse> Handle(
        ProcessObservationFormCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing observation form {FileName} for site {SiteId}, program {ProgramId}, and tracer {TracerId}",
            request.FileName,
            request.SiteId,
            request.ProgramId,
            request.TracerId);

        var processedDocument = await _formRecognizerService.ExtractObservationInfo(request.FileName);

        _logger.LogInformation(
            "Processed observation form {ObservationModel} for site {SiteId}, program {ProgramId}, and tracer {TracerId}",
            processedDocument,
            request.SiteId,
            request.ProgramId,
            request.TracerId);

        var referenceKey = SnowFlakeIdGenerator.NewId().ToString();

        await _stateStore.SaveStateAsync(
            "observations-store",
            referenceKey,
            processedDocument,
            cancellationToken: cancellationToken);

        await _messageBus.PublishEventAsync(
            "jcr-services-bus",
            "EProducts/Services/Observations/ObservationExtracted",
            new ObservationExtractedV1(referenceKey),
            cancellationToken);

        return new ProcessObservationFormResponse(true);
    }
}
