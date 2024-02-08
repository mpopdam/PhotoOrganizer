namespace PhotoOrganizer.Core;

using System.Text.Json;
using System.Text.Json.Serialization;

public class PhotoOrganizeConfig : IPhotoOrganizeConfig
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
    
    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return JsonSerializer.Serialize(this, options);
    }
    
    public static PhotoOrganizeConfig FromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<PhotoOrganizeConfig>(jsonString) ?? new PhotoOrganizeConfig();
    }
}
