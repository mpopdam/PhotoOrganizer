using System.ComponentModel;
using PhotoOrganizer.Console;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Progress = PhotoOrganizer.Core.Progress;

[Description("Organizes photos from the configured [blue]source[/] folder to the configured [blue]target[/] folder, using their date taken to structure them in folders.")]
internal class OrganizeCommand : AsyncCommand<OrganizeCommand.Settings>
{
    private readonly IPhotoOrganizer _photoOrganizer;
    private readonly IConfigFile _configFile;

    public OrganizeCommand(IPhotoOrganizer photoOrganizer, IConfigFile configFile)
    {
        _photoOrganizer = photoOrganizer;
        _configFile = configFile;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings commandSettings)
    {
        var config = _configFile.Read();
        
        string source = GetIfMissing(config.SourceFolder, AskSource);
        string target = GetIfMissing(config.TargetFolder, AskTarget);

        config.UpdateSourceFolder(source);
        config.UpdateTargetFolder(target);

        _configFile.Save(config);

        int count = await _photoOrganizer.GetSourcePhotoCount(source);

        if (count > 0)
        {
            IPhotoOrganizeConfig configFile = _configFile.Read();

            var json = new JsonText(configFile.ToJson());

            AnsiConsole.Write(
                new Panel(json)
                    .Header("Settings")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));

            if (AnsiConsole.Confirm($"About to move [yellow]{count}[/] photos from [blue]{source}[/] to [blue]{target}[/]. Continue?"))
            {
                await Organize(source, target);
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]No photos found in {source} to organize.[/]");
        }

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

    public sealed class Settings : CommandSettings
    {
    }
}
