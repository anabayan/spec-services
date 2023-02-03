using AutoMapper;
using JCR.Services.AppliedAIService.Extract.Features;
using JCR.Services.AppliedAIService.Extract.Features.ExtractingObservation;
using JCR.Services.AppliedAIService.Extract.Features.ProcessingObservationForm;

namespace JCR.Services.AppliedAIService.Extract;

public class ExtractMappers : Profile
{
    public ExtractMappers()
    {
        CreateMap<ExtractObservationRequest, ExtractObservationCommand>()
            .ConstructUsing(req => new ExtractObservationCommand(
                req.File,
                req.SiteId,
                req.ProgramId,
                req.TracerId));

        CreateMap<ObservationFormUploadedEvent, ProcessObservationFormCommand>()
            .ConstructUsing(@event => new ProcessObservationFormCommand(
                @event.FileName,
                @event.SiteId,
                @event.ProgramId,
                @event.TracerId
            ));
    }
}
