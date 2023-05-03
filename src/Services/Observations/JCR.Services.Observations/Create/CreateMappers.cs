using AutoMapper;
using JCR.Services.Observations.Create.Features.UploadingObservation;

namespace JCR.Services.Observations.Create;

public class CreateMappers : Profile
{
    public CreateMappers()
    {
        CreateMap<UploadObservationRequest, UploadObservationCommand>()
            .ConstructUsing(req => new UploadObservationCommand(
                req.File,
                req.SiteId,
                req.ProgramId,
                req.TracerId));
    }
}
