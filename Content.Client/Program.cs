global using Robust.Shared.Analyzers;
global using Robust.Shared.Log;
global using Robust.Shared.IoC;
using Robust.Client;

namespace Content.Client;

internal static class Program
{
    public static void Main(string[] args)
    {
        ContentStart.Start(args);
    }
}
