using Microsoft.Extensions.DependencyInjection;
using PhotoOrganizer.Console;
using PhotoOrganizer.Console.Infrastructure;
using PhotoOrganizer.Core;
using Spectre.Console.Cli;

ServiceCollection services = BuildServices();

// Create a type registrar and register any dependencies. A type registrar is an adapter for a DI framework.
var registrar = new TypeRegistrar(services);

// Create a new command app with the registrar and run it with the provided arguments.
var app = new CommandApp(registrar);

app.Configure(config =>
{
#if DEBUG
    config.PropagateExceptions();
    config.ValidateExamples();
#endif
    
    config.AddExample("config", "show");
    config.AddExample("config", "update", "-s", "C:\\CameraUpload", "-t", "C:\\Photos");
    config.AddExample("organize");
    
    config.AddCommand<OrganizeCommand>("organize");
    config.AddBranch<ConfigSettings>("config", cfg =>
    {
        cfg.AddCommand<ConfigShowCommand>("show");
        cfg.AddCommand<ConfigUpdateCommand>("update");
    });
});

return await app.RunAsync(args);


ServiceCollection BuildServices()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddTransient<IConfigFile, ConfigFile>();
    serviceCollection.AddOrganizerServices();

    return serviceCollection;
}
