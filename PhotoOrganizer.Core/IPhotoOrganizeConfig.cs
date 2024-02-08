namespace PhotoOrganizer.Core;

public interface IPhotoOrganizeConfig
{
    string? SourceFolder { get; }

    string? TargetFolder { get; }

    string[] ImageSearchPatterns { get; }

    string FileFormat { get; }
    
    void UpdateSourceFolder(string sourceFolder);
    
    void UpdateTargetFolder(string targetFolder);

    string ToJson();
}
