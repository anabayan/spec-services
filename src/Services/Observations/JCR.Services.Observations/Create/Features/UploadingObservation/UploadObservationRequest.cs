using BuildingBlocks.Core.Extensions;
using FluentValidation;

namespace JCR.Services.Observations.Create.Features.UploadingObservation;

public record UploadObservationRequest(IFormFile File, int SiteId, int ProgramId, int TracerId)
{
    // Doing this because of this issue: https://github.com/dotnet/aspnetcore/issues/39430
    // Similar model can be followed for IFormFileCollection
    // Won't be needed once the issue is fixed
    // Also won't be needed for non-file use cases
    public static ValueTask<UploadObservationRequest> BindAsync(HttpContext context)
    {
        var form = context.Request.Form;

        // This is a hack until issue 39430 is fixed. Always have validations in validators
        var file = form?.Files?[0] ??
                   throw new ValidationException(
                       "File is required.");

        var siteId = form["SiteId"].FirstOrDefault();
        var programId = form["ProgramId"].FirstOrDefault();
        var tracerId = form["TracerId"].FirstOrDefault();
        var request = new UploadObservationRequest(
            file!,
            siteId.ToInt32(),
            programId.ToInt32(),
            tracerId.ToInt32());

        return new ValueTask<UploadObservationRequest>(request);
    }
}
