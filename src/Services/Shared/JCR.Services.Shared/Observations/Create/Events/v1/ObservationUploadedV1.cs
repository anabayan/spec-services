using BuildingBlocks.Abstractions.Messaging;

namespace JCR.Services.Shared.Observations.Create.Events.v1;

public record ObservationUploadedV1(string FileName, int SiteId, int ProgramId, int TracerId) : IMessage;
