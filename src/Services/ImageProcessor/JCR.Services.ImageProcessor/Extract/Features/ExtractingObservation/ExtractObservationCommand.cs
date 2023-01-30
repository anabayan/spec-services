using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Core.IdsGenerator;
using FluentValidation;

namespace JCR.Services.ImageProcessor.Extract.Features.ExtractingObservation;

public record ExtractObservationCommand(
    IFormFile File,
    int SiteId,
    int ProgramId,
    int TracerId
) : ICommand<ExtractObservationResponse>
{
    public long Id { get; init; } = SnowFlakIdGenerator.NewId();
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
    public Task<ExtractObservationResponse> Handle(
        ExtractObservationCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        // var daprClient = new DaprClientBuilder().Build();

        // await daprClient.InvokeBindingAsync<string>()

        return Task.FromResult(new ExtractObservationResponse(true));
    }
}
