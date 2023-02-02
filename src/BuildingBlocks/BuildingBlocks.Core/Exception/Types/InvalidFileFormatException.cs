namespace BuildingBlocks.Core.Exception.Types;

public class InvalidFileFormatException : BadRequestException
{
    public InvalidFileFormatException(string fileName) : base($"File: '{fileName}' is invalid.")
    {
        FileName = fileName;
    }

    public string FileName { get; }
}
