using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Maths;

namespace Content.Client.GameView;

public sealed class SimulationOverlay : Overlay
{
    public override OverlaySpace Space => OverlaySpace.ScreenSpaceBelowWorld;
    
    protected override void Draw(in OverlayDrawArgs args)
    {
        
    }
}