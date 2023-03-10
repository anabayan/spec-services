using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Core.Extensions;

public static class FileExtensions
{
    public static async Task<string> ToBase64Async(this IFormFile file, CancellationToken cancellationToken)
    {
        var fileBytes = await file.ToByteArrayAsync(cancellationToken);
        return Convert.ToBase64String(fileBytes);
    }

    public static async Task<byte[]> ToByteArrayAsync(this IFormFile file, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        ms.Position = 0;
        return ms.ToArray();
    }
}
