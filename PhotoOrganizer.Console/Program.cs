using Microsoft.Extensions.DependencyInjection;
using PhotoOrganizer.Console.Infrastructure;
using PhotoOrganizer.Core;
using Spectre.Console.Cli;

var services = new ServiceCollection();
services.AddOrganizerServices();

// Create a type registrar and register any dependencies. A type registrar is an adapter for a DI framework.
var registrar = new TypeRegistrar(services);

// Create a new command app with the registrar and run it with the provided arguments.
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.ValidateExamples();

    config.AddExample("organize", "-s", @"C:\\CameraUpload", "-t", "C:\\Photos");
    
    config.AddCommand<OrganizeCommand>("organize");
});

return await app.RunAsync(args);
