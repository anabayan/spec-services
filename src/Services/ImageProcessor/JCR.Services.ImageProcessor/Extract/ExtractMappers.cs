using AutoMapper;
using JCR.Services.ImageProcessor.Extract.Features.ExtractingObservation;

namespace JCR.Services.ImageProcessor.Extract;

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
