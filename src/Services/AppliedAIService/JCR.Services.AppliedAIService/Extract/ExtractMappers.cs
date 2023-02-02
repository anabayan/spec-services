using AutoMapper;
using JCR.Services.AppliedAIService.Extract.Features.ExtractingObservation;

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
    }
}
