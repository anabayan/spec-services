using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Abstractions.Dapr;

public interface IBlobUpload
{
    Task<string> UploadAsync(string blobName, string base64StringContent,
        CancellationToken cancellationToken = default);

    Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
}
