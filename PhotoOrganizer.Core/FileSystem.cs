namespace PhotoOrganizer.Core;

internal class FileSystem : IFileSystem
{
    public Task<string[]> GetFiles(string folder, string[] searchPatterns)
    {
        return Task.Run(() =>
        {
            return searchPatterns
                .SelectMany(pattern => Directory.GetFiles(folder, pattern))
                .Distinct()
                .ToArray();
        });
    }

    public async Task MoveFile(string sourcePath, string targetPath)
    {
        await Task.Run(() =>
        {
            EnsureDirectoryExists(targetPath);

            File.Move(sourcePath, targetPath, false);
        });
    }

    private static void EnsureDirectoryExists(string targetPath)
    {
        string? targetFolderPath = Path.GetDirectoryName(targetPath);
        if ((targetFolderPath != null) && !Directory.Exists(targetFolderPath))
        {
            Directory.CreateDirectory(targetFolderPath);
        }
    }
}
