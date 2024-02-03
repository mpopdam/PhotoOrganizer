namespace PhotoOrganizer.Core;

public interface IFileSystem
{
    Task<string[]> GetFiles(string folder, string[] searchPatterns);
    
    Task MoveFile(string sourcePath, string targetPath);
}
