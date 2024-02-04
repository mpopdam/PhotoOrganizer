namespace PhotoOrganizer.Core;

internal class ImageFileMover : IImageFileMover
{
    private readonly IFileSystem _fileSystem;
    private readonly ISettings _settings;

    public ImageFileMover(IFileSystem fileSystem, ISettings settings)
    {
        _fileSystem = fileSystem;
        _settings = settings;
    }

    public async Task<string> Move(ImageFile image, string targetFolder)
    {
        if (image.DateTaken == null)
        {
            return "";
        }
        
        string targetPath = GetTargetImagePath(targetFolder, image);

        await _fileSystem.MoveFile(image.FilePath, targetPath);
        
        return targetPath;
    }

    private string GetTargetImagePath(string targetFolder, ImageFile image)
    {
        string yearFolder = image.DateTaken!.Value.Year.ToString();
        string monthFolder = image.DateTaken.Value.Month.ToString("00");
        string destinationFolderPath = Path.Combine(targetFolder, yearFolder, monthFolder);
        
        string name = image.DateTaken.Value.ToString(_settings.FileFormat);
        string newFileName = $"{name}{Path.GetExtension(image.FilePath)}";
        
        return Path.Combine(destinationFolderPath, newFileName);
    }
}