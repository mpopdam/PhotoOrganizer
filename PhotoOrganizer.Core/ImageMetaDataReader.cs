namespace PhotoOrganizer.Core;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

internal class ImageMetaDataReader : IImageMetaDataReader
{
    public DateTime? GetDateTaken(string filePath)
    {
        IReadOnlyList<Directory> directories = ImageMetadataReader.ReadMetadata(filePath);

        ExifSubIfdDirectory? directory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
        if (directory is not null)
        {
            int[] dateTags = [ExifDirectoryBase.TagDateTimeOriginal, ExifDirectoryBase.TagDateTimeDigitized];

            foreach (int tag in dateTags)
            {
                if (directory.TryGetDateTime(tag, out DateTime dateTime))
                {
                    return dateTime;
                }
            }
        }

        return null;
    }
}
