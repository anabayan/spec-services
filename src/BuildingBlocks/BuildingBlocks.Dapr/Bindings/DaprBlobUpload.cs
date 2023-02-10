using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.Dapr;
using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Extensions;
using Dapr.Client;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Dapr.Bindings;

public class DaprBlobUpload : IBlobUpload, IFileUploadBinding
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
        var fileArrayContent = await file.ToByteArrayAsync(cancellationToken);

        Guard.Against.InvalidFile(fileArrayContent, file.FileName);

        // TODO: This is a temporary fix for the issue with spaces in the file name.
        // Update with JCR policy naming convention.
        var fileName = file.FileName.Replace(" ", "_", StringComparison.OrdinalIgnoreCase);

        await _daprClient.InvokeBindingAsync(
            "observation-extraction-output-binding",
            "create",
            Convert.ToBase64String(fileArrayContent),
            new Dictionary<string, string> { { "blobName", $"{fileName}" } },
            cancellationToken);

        return file.FileName;
    }
}
