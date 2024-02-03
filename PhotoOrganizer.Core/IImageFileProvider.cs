namespace PhotoOrganizer.Core;

public interface IImageFileProvider
{
    Task<ImageFile[]> GetImageFiles(string sourceFolder, string[] searchPatterns);
}
