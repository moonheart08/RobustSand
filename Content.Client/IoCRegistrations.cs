using Content.Client.Simulation;
using Content.Client.UI;

namespace Content.Client;

internal static class IoCRegistrations
{
    public static void Register()
    {
        IoCManager.Register<StylesheetManager>();
    }
}