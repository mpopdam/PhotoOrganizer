using System.ComponentModel;
using PhotoOrganizer.Console;
using PhotoOrganizer.Core;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using Progress = PhotoOrganizer.Core.Progress;

[Description(
    "Organizes photos from the configured [blue]source[/] folder to the configured [blue]target[/] folder, using their date taken to structure them in folders.")]
internal class OrganizeCommand : AsyncCommand<OrganizeCommand.Settings>
{
    private readonly IConfigFile _configFile;
    private readonly IPhotoOrganizer _photoOrganizer;

    public OrganizeCommand(IPhotoOrganizer photoOrganizer, IConfigFile configFile)
    {
        _photoOrganizer = photoOrganizer;
        _configFile = configFile;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings commandSettings)
    {
        IPhotoOrganizeConfig config = _configFile.Read();

        string source = GetIfMissing(config.SourceFolder, AskSource);
        string target = GetIfMissing(config.TargetFolder, AskTarget);

        config.UpdateSourceFolder(source);
        config.UpdateTargetFolder(target);

        _configFile.Save(config);

        await Organize(config);

        return 0;
    }

    private string GetIfMissing(string? value, Func<string> ask) => string.IsNullOrEmpty(value) ? ask() : value;

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

    private async Task Organize(IPhotoOrganizeConfig config)
    {
        string source = config.SourceFolder!;
        string target = config.TargetFolder!;

        int count = await _photoOrganizer.GetSourcePhotoCount(source, config.ImageSearchPatterns);

        if (count > 0)
        {
            var json = new JsonText(config.ToJson());

            AnsiConsole.Write(
                new Panel(json)
                    .Header("Settings")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));

            if (AnsiConsole.Confirm(
                    $"About to move [yellow]{count}[/] photos from [blue]{source}[/] to [blue]{target}[/]. Continue?"))
            {
                int successCount = 0;
                int failureCount = 0;
                
                _photoOrganizer.ProgressChanged += (_, progress) =>
                {
                    successCount += progress.LastMoveResult?.Status == FileMoveStatus.Success ? 1 : 0;
                    failureCount += progress.LastMoveResult?.Status != FileMoveStatus.Success ? 1 : 0;
                    
                    LogProgress(progress);
                };

                await _photoOrganizer.OrganizePhotos(source, target, config.ImageSearchPatterns, config.FileFormat);
                
                AnsiConsole.WriteLine();
                
                if (failureCount == 0)
                {
                    AnsiConsole.MarkupLine($"[green]Moved {successCount} photo(s) successfully.[/]");                
                }
                else
                {
                    if (successCount > 0)
                    {
                        AnsiConsole.MarkupLine($"Moved [green]{successCount}[/] photo(s) successfully.");                
                    }
                    AnsiConsole.MarkupLine($"Failed to move [red]{failureCount}[/] photo(s).");
                }
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]No photos found in {source}.[/]");
        }
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
