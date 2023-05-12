using BuildingBlocks.Abstractions.Messaging;

namespace JCR.Services.Shared.Observations.Create.Events.v1;

public record ObservationExtractedV1(string key) : IMessage;
