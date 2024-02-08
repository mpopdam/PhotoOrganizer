namespace PhotoOrganizer.Console;

using Core;

public class ConfigFile : IConfigFile
{
    private static readonly string s_filePath = Path.Combine(AppContext.BaseDirectory, "config.json");

    public IPhotoOrganizeConfig Read()
    {
        if (!File.Exists(s_filePath))
        {
            return new PhotoOrganizeConfig();
        }

        string jsonString = File.ReadAllText(s_filePath);
        return PhotoOrganizeConfig.FromJson(jsonString);
    }

    public void Save(IPhotoOrganizeConfig config)
    {
        File.WriteAllText(s_filePath, config.ToJson());
    }
}