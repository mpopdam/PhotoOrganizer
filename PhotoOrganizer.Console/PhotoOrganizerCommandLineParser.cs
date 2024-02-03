namespace PhotoOrganizer.Console;

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Core;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

public class PhotoOrganizerCommandLineParser
{
    private readonly Parser _parser;

    public PhotoOrganizerCommandLineParser()
    {
        ServiceProvider serviceProvider = BuildServiceProvider();

        OrganizeCommand organizeCommand = serviceProvider.GetRequiredService<OrganizeCommand>();

        RootCommand rootCommand = new("Photo Organizer")
        {
            organizeCommand.Build()
        };

        var commandLineBuilder = new CommandLineBuilder(rootCommand);
        commandLineBuilder
            .UseDefaults()
            .UseHelp(ctx =>
            {
                ctx.HelpBuilder.CustomizeLayout(
                    _ =>
                        HelpBuilder.Default
                            .GetLayout()
                            .Skip(1) // Skip the default command description section.
                            .Prepend(_ =>
                            {
                                AnsiConsole.Write(new FigletText(rootCommand.Description!).LeftJustified().Color(Color.Blue));
                                AnsiConsole.WriteLine();
                            }));
            });

        _parser = commandLineBuilder.Build();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddOrganizerServices();

        services.AddTransient<OrganizeCommand>();

        return services.BuildServiceProvider();
    }

    public Task<int> InvokeAsync(string[] args) => _parser.InvokeAsync(args);
}