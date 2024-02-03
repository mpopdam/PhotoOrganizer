namespace PhotoOrganizer.Core;

public class FileMoveResult
{
    public FileMoveResult(ImageFile sourceFile, string targetFilePath = "")
    {
        SourceFile = sourceFile;
        TargetFilePath = targetFilePath;
        Status = targetFilePath != "" ? FileMoveStatus.Success : FileMoveStatus.GenericFailure;
    }

    public ImageFile SourceFile { get; }
    public string TargetFilePath { get; }
    public FileMoveStatus Status { get; init; }
}