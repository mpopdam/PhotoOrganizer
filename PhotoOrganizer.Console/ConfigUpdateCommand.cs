using System.ComponentModel;
using PhotoOrganizer.Console;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

[Description("Updates the configuration in the config JSON file.")]
internal class ConfigUpdateCommand : Command<ConfigUpdateCommand.Settings>, ICommandLimiter<ConfigSettings>
{
    private readonly IConfigFile _configFile;

    public ConfigUpdateCommand(IConfigFile configFile)
    {
        _configFile = configFile;
    }

    public override int Execute(CommandContext context, Settings commandSettings)
    {
        string source = GetIfMissing(commandSettings.Source, AskSource);
        string target = GetIfMissing(commandSettings.Target, AskTarget);
       
        IPhotoOrganizeConfig photoOrganizeConfig = _configFile.Read();
        photoOrganizeConfig.UpdateSourceFolder(source);
        photoOrganizeConfig.UpdateTargetFolder(target);
        
        _configFile.Save(photoOrganizeConfig);
        
        var json = new JsonText(photoOrganizeConfig.ToJson());

        AnsiConsole.Write(
            new Panel(json)
                .Header("Settings")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

        return 0;
    }

    private string GetIfMissing(string? value, Func<string> ask)
    {
        return string.IsNullOrEmpty(value) ? ask() : value;
    }

    private string AskSource() =>
        AnsiConsole.Prompt(new TextPrompt<string>("Source folder:")
            .Validate(s => !string.IsNullOrWhiteSpace(s) && Directory.Exists(s)
                ? ValidationResult.Success()
                : ValidationResult.Error("The source folder is required and must exist.")));

    private string AskTarget() =>
        AnsiConsole.Prompt(new TextPrompt<string>("Target folder:")
            .Validate(s => !string.IsNullOrWhiteSpace(s)
                ? ValidationResult.Success()
                : ValidationResult.Error("The source folder is required.")));

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-s|--source")]
        [Description("The folder containing the source files to organize.")]
        public string Source { get; set; } = "";

        [CommandOption("-t|--target")]
        [Description("The target folder where to move the files.")]
        public string Target { get; set; } = "";
    }
}
