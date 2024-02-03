namespace PhotoOrganizer.Tests;

using Core;
using FakeItEasy;
using FluentAssertions;

public class PhotoOrganizerTests
{
    private const string sourceFolder = @"C:\Source";
    private const string targetFolder = @"C:\Target";

    [Fact]
    public void When_there_are_no_image_files_in_the_source_folder_it_should_not_do_anything()
    {
        // Arrange
        string[] searchPatterns = ["*.jpg"];
        IImageFileProvider fileProvider = A.Fake<IImageFileProvider>();
        A.CallTo(() => fileProvider.GetImageFiles(sourceFolder, searchPatterns)).Returns([]);

        IImageFileMover fileMover = A.Fake<IImageFileMover>();

        bool eventWasRaised = false;

        var organizer = new PhotoOrganizer(fileProvider, fileMover);
        organizer.ProgressChanged += (_, _) => { eventWasRaised = true; };

        // Act
        organizer.OrganizePhotos(sourceFolder, targetFolder);
        A.CallTo(() => fileMover.Move(A<ImageFile>._, A<string>._)).MustNotHaveHappened();

        eventWasRaised.Should().BeFalse();
    }

    [Fact]
    public void When_there_is_an_image_file_in_the_source_folder_it_should_move_the_file_to_the_correct_folder_and_report_progress()
    {
        // Arrange
        var image = new ImageFile(Path.Combine(sourceFolder, "someImage.jpg"));

        string[] searchPatterns = ["*.jpg"];
        IImageFileProvider fileProvider = A.Fake<IImageFileProvider>();
        A.CallTo(() => fileProvider.GetImageFiles(sourceFolder, searchPatterns)).Returns([image]);

        IImageFileMover fileMover = A.Fake<IImageFileMover>();

        var actualProgress = default(Progress);
        var organizer = new PhotoOrganizer(fileProvider, fileMover);
        organizer.ProgressChanged += (_, progress) =>
        {
            actualProgress = progress;
        };

        // Act
        organizer.OrganizePhotos(sourceFolder, targetFolder);

        // Assert
        A.CallTo(() => fileMover.Move(image, targetFolder)).MustHaveHappenedOnceExactly();
        
        actualProgress.Should().NotBeNull();
        actualProgress!.Current.Should().Be(1);
        actualProgress.Total.Should().Be(1);
        actualProgress.Percentage.Should().Be(100);
    }
}