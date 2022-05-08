using Robust.Client.UserInterface;
using Content.Client.Simulation;
using Robust.Client.Graphics;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Robust.Shared.Maths.Color;

namespace Content.Client.Rendering;

public sealed partial class SimulationControl : Control
{
    [Dependency] private readonly IClyde _clyde = default!;

    public float UpdateRate = 1.0f / 30;

    public float Accumulator = 0;

    public Simulation.Simulation Simulation = new Simulation.Simulation();

    private OwnedTexture _renderBuffer;

    public SimulationControl()
    {
        IoCManager.InjectDependencies(this);
        AlwaysRender = true;
        _renderBuffer = _clyde.CreateBlankTexture<Rgba32>(
            new Vector2i((int) SimulationConfig.SimWidth, (int) SimulationConfig.SimHeight), "simbuffer",
            TextureLoadParameters.Default);
        _bufferClear = new Image<Rgba32>(_renderBuffer.Width, _renderBuffer.Height, new Rgba32(0, 0, 0));
    }

    public void Update()
    {
        // Run a single sim frame.
        Simulation.RunFrame();
        DrawNewFrame();
    }
    
    protected override void KeyBindDown(GUIBoundKeyEventArgs args)
    {
        base.KeyBindDown(args);
    }

    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        base.KeyBindUp(args);
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        var rect = UIBox2
            .FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight))
            .Scale(UIScale);
        handle.DrawTextureRect(_renderBuffer, rect);
        MinSize = rect.Size;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        Accumulator += args.DeltaSeconds;
        if (Accumulator > UpdateRate)
        {
            Accumulator -= UpdateRate;
            Update();
        }
    }
}