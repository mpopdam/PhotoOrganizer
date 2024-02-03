namespace PhotoOrganizer.Core;

public class ImageFile : IImageFile
{
    public ImageFile(string filePath)
    {
        FilePath = filePath;
        DateTaken = null;
    }

    public string FilePath { get; }

    public DateTime? DateTaken { get; init; }

    public override string ToString() => $"File: {FilePath}, Date Taken: {DateTaken}";
}