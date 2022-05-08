using Content.Client.Maps;
using Content.Client.UI;

namespace Content.Client;

internal static class IoCRegistrations
{
    public static void Register()
    {
        IoCManager.Register<StylesheetManager>();
        IoCManager.Register<GameMapManager>();
    }
}