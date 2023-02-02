using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Core.Extensions;
using Dapr.Client;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Dapr.Bindings;

public class DaprBlobUpload : IBlobUpload, IDaprService
{
    private readonly DaprClient _daprClient;

    public DaprBlobUpload(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<string> UploadAsync(
        string blobName,
        string base64StringContent,
        CancellationToken cancellationToken = default)
    {
        await _daprClient.InvokeBindingAsync(
            "observation-extraction-output-binding",
            "create",
            base64StringContent,
            new Dictionary<string, string> { { "blobName", $"{blobName}" } },
            cancellationToken);

        return blobName;
    }

    public async Task<string> UploadAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var fileBase64StringContent = await file.ToBase64Async(cancellationToken);

        await _daprClient.InvokeBindingAsync(
            "observation-extraction-output-binding",
            "create",
            fileBase64StringContent,
            new Dictionary<string, string> { { "blobName", $"{file.FileName}" } },
            cancellationToken);

        return file.FileName;
    }
}
