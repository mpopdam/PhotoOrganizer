namespace PhotoOrganizer.Core;

public interface IPhotoOrganizer
{
    Task OrganizePhotos(string sourceFolder, string targetFolder);

    event EventHandler<Progress> ProgressChanged;
}
