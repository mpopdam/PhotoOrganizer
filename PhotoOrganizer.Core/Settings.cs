namespace PhotoOrganizer.Core;

internal class Settings : ISettings
{
    public string SourceFolder { get; init; } = "C:\\Projects\\PhotoOrganizer\\Source";

    public string TargetFolder { get; init; } = "C:\\Projects\\PhotoOrganizer\\Target";

    public string[] ImageSearchPatterns { get; init; } = ["*.jpg", "*.jpeg", "*.tiff"];

    public string FileFormat { get; set; } = "yyyyMMdd_HH_mm_ss";
}
