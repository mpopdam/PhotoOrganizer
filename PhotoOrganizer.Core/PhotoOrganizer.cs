namespace PhotoOrganizer.Core;

internal class PhotoOrganizer : IPhotoOrganizer
{
    private readonly IImageFileMover _fileMover;
    private readonly IImageFileProvider _fileProvider;

    public PhotoOrganizer(IImageFileProvider fileProvider, IImageFileMover fileMover)
    {
        _fileProvider = fileProvider;
        _fileMover = fileMover;
    }

    public event EventHandler<Progress>? ProgressChanged;

    public async Task<int> GetSourcePhotoCount(string sourceFolder, string[] imageSearchPatterns)
    {
        ImageFile[] imageFiles = await _fileProvider.GetImageFiles(sourceFolder, imageSearchPatterns);
        
        return imageFiles.Length;
    }

    public async Task OrganizePhotos(string sourceFolder, string targetFolder, string[] imageSearchPatterns, string fileFormat)
    {
        ImageFile[] imageFiles = await _fileProvider.GetImageFiles(sourceFolder, imageSearchPatterns);
        var progress = new Progress(imageFiles.Length);

        foreach (ImageFile imageFile in imageFiles)
        {
            FileMoveResult? result;
            
            if (imageFile.DateTaken == null)
            {
                result = new FileMoveResult(imageFile)
                {
                    Status = FileMoveStatus.NoDateTaken
                };
            }
            else
            {
                try
                {
                    string targetPath = await _fileMover.Move(imageFile, targetFolder, fileFormat);
                    result = new FileMoveResult(imageFile, targetPath);
                }
                catch (IOException)
                {
                    result = new FileMoveResult(imageFile)
                    {
                        Status = FileMoveStatus.TargetExists
                    };
                }
            }
                
            progress.Update(result);
            OnProgressChanged(progress);
        }
    }

    private void OnProgressChanged(Progress progress) => ProgressChanged?.Invoke(this, progress);
}