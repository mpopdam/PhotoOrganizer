namespace PhotoOrganizer.Console.Infrastructure;

using Spectre.Console.Cli;

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public object? Resolve(Type? type)
    {
        return type == null ? null : _provider.GetService(type);

    }
}
