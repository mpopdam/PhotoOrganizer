namespace PhotoOrganizer.Core;

public interface ISettings
{
    string SourceFolder { get; }

    string TargetFolder { get; }

    string[] ImageSearchPatterns { get; }

    string FileFormat { get; }
}
