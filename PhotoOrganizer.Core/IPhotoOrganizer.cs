namespace PhotoOrganizer.Core;

public interface IPhotoOrganizer
{
    Task<int> GetSourcePhotoCount(string sourceFolder, string[] imageSearchPatterns);

    Task OrganizePhotos(string sourceFolder, string targetFolder, string[] imageSearchPatterns, string fileFormat);

    event EventHandler<Progress> ProgressChanged;
}
