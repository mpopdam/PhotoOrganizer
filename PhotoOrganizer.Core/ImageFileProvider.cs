namespace PhotoOrganizer.Core;

using MetadataExtractor;

internal class ImageFileProvider : IImageFileProvider
{
    private readonly IImageMetaDataReader _imageMetaDataReader;
    private readonly IFileSystem _fileSystem;

    public ImageFileProvider(IFileSystem fileSystem, IImageMetaDataReader imageMetaDataReader)
    {
        _fileSystem = fileSystem;
        _imageMetaDataReader = imageMetaDataReader;
    }

    public async Task<ImageFile[]> GetImageFiles(string sourceFolder, string[] searchPatterns)
    {
        string[] imageFilePaths = await _fileSystem.GetFiles(sourceFolder, searchPatterns);

        return imageFilePaths
            .Select(ConvertToImageFile)
            .ToArray();
    }

    private ImageFile ConvertToImageFile(string filePath)
    {
        return new ImageFile(filePath)
        {
            DateTaken = _imageMetaDataReader.GetDateTaken(filePath)
        };
    }
}
