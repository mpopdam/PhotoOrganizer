using System.CommandLine;
using PhotoOrganizer.Core;
using Spectre.Console;
using Progress = PhotoOrganizer.Core.Progress;

internal class OrganizeCommand
{
    private readonly IPhotoOrganizer _photoOrganizer;

    public OrganizeCommand(IPhotoOrganizer photoOrganizer)
    {
        _photoOrganizer = photoOrganizer;
    }

    public Command Build()
    {
        var sourceFolderOption = new Option<string>("--source", "The folder containing the source files to organize")
        {
            IsRequired = true
        };

        sourceFolderOption.AddAlias("-s");

        var targetFolderOption = new Option<string>("--target", "The target folder where to move the files")
        {
            IsRequired = true
        };

        targetFolderOption.AddAlias("-t");

        var command = new Command("organize", "Organizes the images in the source folder by their date/time taken")
        {
            sourceFolderOption,
            targetFolderOption
        };

        command.SetHandler(async context =>
        {
            string sourceFolder = context.ParseResult.GetValueForOption(sourceFolderOption) ?? "";
            string targetFolder = context.ParseResult.GetValueForOption(targetFolderOption) ?? "";

            await Organize(sourceFolder, targetFolder);
        });

        return command;
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

        FileMoveResult result = progress.LastMoveResult;

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
