using Robust.Shared.Input;

namespace Content.Client.Input;

public static class ContentContexts
{
    public static void SetupContexts(IInputContextContainer contexts)
    {
        var common = contexts.GetContext("editor");
        common.AddFunction(ContentKeyFunctions.BrushSizeUp);
        common.AddFunction(ContentKeyFunctions.BrushSizeDown);
    }
}