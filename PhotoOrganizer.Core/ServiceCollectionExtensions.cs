namespace PhotoOrganizer.Core;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddOrganizerServices(this IServiceCollection services)
    {
        services.AddSingleton<IPhotoOrganizeSettings, PhotoOrganizeSettings>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<IImageMetaDataReader, ImageMetaDataReader>();
        
        services.AddTransient<IImageFileProvider, ImageFileProvider>();
        services.AddTransient<IImageFileMover, ImageFileMover>();
        services.AddTransient<IPhotoOrganizer, PhotoOrganizer>();
    }
}
