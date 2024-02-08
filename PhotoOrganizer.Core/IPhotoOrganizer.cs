namespace PhotoOrganizer.Core;

public interface IPhotoOrganizer
{
    Task<int> GetSourcePhotoCount(string sourceFolder);

    Task OrganizePhotos(string sourceFolder, string targetFolder);

    event EventHandler<Progress> ProgressChanged;
}
