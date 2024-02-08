using System.ComponentModel;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Progress = PhotoOrganizer.Core.Progress;

internal class OrganizeCommand : AsyncCommand<OrganizeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-s|--source")]
        [Description("The folder containing the source files to organize.")]
        public string Source { get; set; } = "";

        [CommandOption("-t|--target")]
        [Description("The target folder where to move the files.")]
        public string Target { get; set; } = "";
    }

    private readonly IPhotoOrganizer _photoOrganizer;
    private readonly IPhotoOrganizeSettings _photoOrganizeSettings;

    public OrganizeCommand(IPhotoOrganizer photoOrganizer, IPhotoOrganizeSettings photoOrganizeSettings)
    {
        _photoOrganizer = photoOrganizer;
        _photoOrganizeSettings = photoOrganizeSettings;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        string source = AskSourceIfMissing(settings.Source);
        string target = AskTargetIfMissing(settings.Target);
        
        await Organize(source, target);

        return 0;
    }

    private string AskSourceIfMissing(string? current)
    {
        return string.IsNullOrEmpty(current)
            ? AnsiConsole.Prompt(new TextPrompt<string>("Source folder:")
                .Validate(s => !string.IsNullOrWhiteSpace(s) && Directory.Exists(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("The source folder is required and must exist.")))
            : current;
    }

    private string AskTargetIfMissing(string? current)
    {
        return string.IsNullOrEmpty(current)
            ? AnsiConsole.Prompt(new TextPrompt<string>("Target folder:")
                .Validate(s => !string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("The source folder is required.")))
            : current;
    }

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
        string progressIndication = $"{progress.Current.ToString().PadLeft(progress.Total.ToString().Length, '0')} / {progress.Total}";

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
}
