using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client.Editor;

public sealed class ViewBackgroundOverlay : Overlay
{
    public override OverlaySpace Space => OverlaySpace.ScreenSpaceBelowWorld;
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        args.DrawingHandle.DrawCircle(Vector2.Zero, 64f, Color.Aqua);
    }
}