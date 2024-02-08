namespace PhotoOrganizer.Core;

public class PhotoOrganizeSettings : IPhotoOrganizeSettings
{
    public string? SourceFolder { get; set; }

    public string? TargetFolder { get; set; }

    public string[] ImageSearchPatterns { get; init; } = ["*.jpg", "*.jpeg", "*.tiff"];

    public string FileFormat { get; set; } = "yyyyMMdd_HH_mm_ss";

    public void UpdateSourceFolder(string sourceFolder)
    {
        SourceFolder = sourceFolder;
    }

    public void UpdateTargetFolder(string targetFolder)
    {
        TargetFolder = targetFolder;        
    }
}
