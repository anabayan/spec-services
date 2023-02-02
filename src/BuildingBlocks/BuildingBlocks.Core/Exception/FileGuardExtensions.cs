using Ardalis.GuardClauses;
using BuildingBlocks.Core.Exception.Types;

namespace BuildingBlocks.Core.Exception;

public static class FileGuardExtensions
{
    private static readonly Dictionary<string, List<byte[]>> _fileSignatures = new()
    {
        { ".DOC", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
        { ".DOCX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        { ".PDF", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
        {
            ".ZIP",
            new List<byte[]>
            {
                new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                new byte[] { 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x55 },
                new byte[] { 0x50, 0x4B, 0x53, 0x70, 0x58 },
                new byte[] { 0x50, 0x4B, 0x05, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
                new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 }
            }
        },
        { ".PNG", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        {
            ".JPG",
            new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
            }
        },
        {
            ".JPEG",
            new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
            }
        },
        {
            ".XLS",
            new List<byte[]>
            {
                new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 },
                new byte[] { 0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00 },
                new byte[] { 0xFD, 0xFF, 0xFF, 0xFF }
            }
        },
        { ".XLSX", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        { ".GIF", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } }
    };

    private static bool IsValidFileExtension(byte[] fileData, string fileName, byte[] allowedChars = null)
    {
        if (string.IsNullOrEmpty(fileName) || fileData == null || fileData.Length == 0) return false;

        var flag = false;
        var ext = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(ext)) return false;

        ext = ext.ToUpperInvariant();

        // Checks for ASCII characters only
        if (ext.Equals(".TXT") || ext.Equals(".CSV") || ext.Equals(".PRN"))
        {
            foreach (var b in fileData)
                if (b > 0x7F)
                {
                    if (allowedChars != null)
                    {
                        if (!allowedChars.Contains(b)) return false;
                    }
                    else
                    {
                        return false;
                    }
                }

            return true;
        }

        if (!_fileSignatures.ContainsKey(ext)) return true;

        var sig = _fileSignatures[ext];
        foreach (var b in sig)
        {
            var curFileSig = new byte[b.Length];
            Array.Copy(fileData, curFileSig, b.Length);
            if (curFileSig.SequenceEqual(b))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    /// <summary>
    ///     Guards against corrupt files or files with invalid/incorrect extensions
    /// </summary>
    public static string InvalidFile(this IGuardClause guardClause, byte[] fileContent, string fileName)
    {
        return guardClause.InvalidFile(fileContent, fileName, new InvalidFileFormatException(fileName));
    }

    /// <summary>
    ///     Guards against corrupt files or files with invalid/incorrect extensions
    /// </summary>
    public static string InvalidFile(this IGuardClause guardClause, byte[] fileContent, string fileName,
        System.Exception exception)
    {
        if (!IsValidFileExtension(fileContent, fileName)) throw exception;

        return fileName;
    }
}
