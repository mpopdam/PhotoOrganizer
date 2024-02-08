using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Progress = PhotoOrganizer.Core.Progress;

internal class OrganizeCommand : AsyncCommand<OrganizeCommand.Settings>
{
    private static readonly string s_settingsFilePath = Path.Combine(AppContext.BaseDirectory, "settings.json");

    private readonly IPhotoOrganizer _photoOrganizer;
    private readonly IPhotoOrganizeSettings _photoOrganizeSettings;

    public OrganizeCommand(IPhotoOrganizer photoOrganizer, IPhotoOrganizeSettings photoOrganizeSettings)
    {
        _photoOrganizer = photoOrganizer;
        _photoOrganizeSettings = photoOrganizeSettings;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings commandSettings)
    {
        string GetIfMissing(string? commandSettingValue, string? fileSettingValue, Func<string> ask)
        {
            return string.IsNullOrEmpty(commandSettingValue)
                ? fileSettingValue ?? ask()
                : commandSettingValue;
        }

        LoadSettings(_photoOrganizeSettings);
        
        string source = GetIfMissing(commandSettings.Source, _photoOrganizeSettings.SourceFolder, AskSource);
        string target = GetIfMissing(commandSettings.Target, _photoOrganizeSettings.TargetFolder, AskTarget);

        _photoOrganizeSettings.UpdateSourceFolder(source);
        _photoOrganizeSettings.UpdateTargetFolder(target);

        SaveSettings(_photoOrganizeSettings);

        await Organize(source, target);

        return 0;
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

    private async Task Organize(string sourceFolder, string targetFolder)
    {
        _photoOrganizer.ProgressChanged += (_, progress) =>
        {
            LogProgress(progress);
        };

        await _photoOrganizer.OrganizePhotos(sourceFolder, targetFolder);
    }

    private static void LogProgress(Progress progress)
    {
        string progressIndication =
            $"{progress.Current.ToString().PadLeft(progress.Total.ToString().Length, '0')} / {progress.Total}";

        FileMoveResult? result = progress.LastMoveResult;
        if (result == null)
        {
            return;
        }

        string statusIndication = result.Status switch
        {
            FileMoveStatus.Success => "[green]V[/]",
            FileMoveStatus.GenericFailure => "[red]X[/]",
            _ => "[yellow]!![/]"
        };

        string sourceFile = Path.GetFileName(result.SourceFile.FilePath);

        string fileInfo = result.Status switch
        {
            FileMoveStatus.Success => $"{sourceFile} [blue]->[/] {result.TargetFilePath}",
            FileMoveStatus.NoDateTaken => $"[blue]{sourceFile}[/] does not have a date taken",
            FileMoveStatus.TargetExists => $"Target for [blue]{sourceFile}[/] ({result.SourceFile.DateTaken:s}) already exists",
            _ => $"{sourceFile} could not be moved to {result.TargetFilePath}"
        };

        AnsiConsole.MarkupLine($"{progressIndication} - {statusIndication} {fileInfo}");
    }

    private void LoadSettings(IPhotoOrganizeSettings photoOrganizeSettings)
    {
        if (!File.Exists(s_settingsFilePath))
        {
            return;
        }

        string jsonString = File.ReadAllText(s_settingsFilePath);
        var settings = JsonSerializer.Deserialize<PhotoOrganizeSettings>(jsonString);

        if (settings == null)
        {
            return;
        }
        
        photoOrganizeSettings.UpdateSourceFolder(settings.SourceFolder);
        photoOrganizeSettings.UpdateTargetFolder(settings.TargetFolder);
    }

    private void SaveSettings(IPhotoOrganizeSettings settings)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string jsonString = JsonSerializer.Serialize(settings, options);

        File.WriteAllText(s_settingsFilePath, jsonString);
    }

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
