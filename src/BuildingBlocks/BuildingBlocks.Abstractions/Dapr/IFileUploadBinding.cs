using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Abstractions.Dapr;

public interface IFileUploadBinding : IDaprService
{
    public Task<string> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);
}
