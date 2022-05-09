using Robust.Shared.Input;

namespace Content.Client.Input;

[KeyFunctions]
public static class ContentKeyFunctions
{
    // DEVNOTE: Stick keys you want to be bindable here.
    // public static readonly BoundKeyFunction DummyKey = "DummyKey";
    public static readonly BoundKeyFunction BrushSizeUp = new BoundKeyFunction("BrushSizeUp");
    public static readonly BoundKeyFunction BrushSizeDown = new BoundKeyFunction("BrushSizeDown");
}