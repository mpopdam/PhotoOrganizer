namespace PhotoOrganizer.Core;

public interface IPhotoOrganizeSettings
{
    string SourceFolder { get; }

    string TargetFolder { get; }

    string[] ImageSearchPatterns { get; }

    string FileFormat { get; }
}