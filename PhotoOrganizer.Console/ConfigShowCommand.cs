using System.ComponentModel;
using PhotoOrganizer.Console;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

[Description("Shows the configuration from the config JSON file.")]
internal class ConfigShowCommand : Command<ConfigShowCommand.Settings>, ICommandLimiter<ConfigSettings>
{
    private readonly IConfigFile _configFile;

    public ConfigShowCommand(IConfigFile configFile)
    {
        _configFile = configFile;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        IPhotoOrganizeConfig configFile = _configFile.Read();

        var json = new JsonText(configFile.ToJson());

        AnsiConsole.Write(
            new Panel(json)
                .Header("Settings")
                .Collapse()
                .RoundedBorder()
                .BorderColor(Color.Yellow));

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
    }
}