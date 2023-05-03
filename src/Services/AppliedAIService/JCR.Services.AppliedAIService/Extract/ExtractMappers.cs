using AutoMapper;
using JCR.Services.AppliedAIService.Extract.Features.ProcessingObservationForm;
using JCR.Services.Shared.Observations.Create.Events.v1;

namespace JCR.Services.AppliedAIService.Extract;

public class ExtractMappers : Profile
{
    public ExtractMappers()
    {
        CreateMap<ObservationUploadedV1, ProcessObservationFormCommand>()
            .ConstructUsing(@event => new ProcessObservationFormCommand(
                @event.FileName,
                @event.SiteId,
                @event.ProgramId,
                @event.TracerId
            ));
    }
}
