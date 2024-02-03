namespace PhotoOrganizer.Core;

public interface IImageMetaDataReader
{       
    DateTime? GetDateTaken(string filePath);
}