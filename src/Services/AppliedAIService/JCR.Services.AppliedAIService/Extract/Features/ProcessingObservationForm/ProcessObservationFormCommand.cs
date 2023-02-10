using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.IdsGenerator;
using FluentValidation;

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

public class
    ProcessObservationCommandHandler : ICommandHandler<ProcessObservationFormCommand, ProcessObservationFormResponse>
{
    private readonly ILogger<ProcessObservationCommandHandler> _logger;
    private readonly ISerializer _serializer;

    public ProcessObservationCommandHandler(ILogger<ProcessObservationCommandHandler> logger, ISerializer serializer)
    {
        _serializer = serializer;
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

        var processedDocument = await FormRecognizer.Recognize(request.FileName);

        _logger.LogInformation(
            @"Processed observation form {ObservationModel} for site {SiteId}, program {ProgramId}, and tracer {TracerId}",
            _serializer.Serialize(processedDocument),
            request.SiteId,
            request.ProgramId,
            request.TracerId);

        return new ProcessObservationFormResponse(true);
    }
}
