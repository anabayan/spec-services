namespace JCR.Services.AppliedAIService.Extract.Features;

public record ObservationFormUploadedEvent
    (string FileName, int SiteId, int ProgramId, int TracerId);
