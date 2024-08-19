namespace PhotoOrganizer.Core;

internal class ImageFileMover : IImageFileMover
{
    private readonly IFileSystem _fileSystem;

    public ImageFileMover(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<string> Move(ImageFile image, string targetFolder, string fileFormat)
    {
        if (image.DateTaken == null)
        {
            return "";
        }
        
        string targetPath = GetTargetImagePath(targetFolder, image, fileFormat);

        await _fileSystem.MoveFile(image.FilePath, targetPath);
        
        return targetPath;
    }

    private string GetTargetImagePath(string targetFolder, ImageFile image, string fileFormat)
    {
        string yearFolder = image.DateTaken!.Value.Year.ToString();
        string monthFolder = image.DateTaken.Value.Month.ToString("00");
        string destinationFolderPath = Path.Combine(targetFolder, yearFolder, monthFolder);
        
        string name = image.DateTaken.Value.ToString(fileFormat);
        string newFileName = $"{name}{Path.GetExtension(image.FilePath)}";
        
        return Path.Combine(destinationFolderPath, newFileName);
    }
}