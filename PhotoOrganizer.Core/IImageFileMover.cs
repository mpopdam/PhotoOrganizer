namespace PhotoOrganizer.Core;

public interface IImageFileMover
{
    Task<string> Move(ImageFile image, string targetFolder);
}