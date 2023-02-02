using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Core.Extensions;
using BuildingBlocks.Core.IdsGenerator;
using Dapr.Client;
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
    private readonly DaprClient _daprClient;

    public ExtractObservationHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<ExtractObservationResponse> Handle(
        ExtractObservationCommand request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var fileBase64StringContent = await request.File.ToBase64Async(cancellationToken);

        await _daprClient.InvokeBindingAsync(
            "observation-extraction-output-binding",
            "create",
            fileBase64StringContent,
            new Dictionary<string, string> { { "blobName", $"{request.File.FileName}" } },
            cancellationToken);

        return await Task.FromResult(new ExtractObservationResponse(true));
    }
}
