namespace PhotoOrganizer.Console;

using Core;

public interface IConfigFile
{
    IPhotoOrganizeConfig Read();

    void Save(IPhotoOrganizeConfig config);
}
