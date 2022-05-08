using System.Threading.Tasks;
using Robust.Client.UserInterface;
using Content.Client.Simulation;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Robust.Shared.Maths.Color;

namespace Content.Client.Rendering;

public sealed partial class SimulationControl : Control
{
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    public float UpdateRate = 1.0f / 30;

    public float Accumulator = 0;

    public SimulationSystem _simSys;

    public Vector2i MousePosition;

    private bool currentlyDrawing = false;

    private OwnedTexture _renderBuffer;

    public SimulationControl()
    {
        IoCManager.InjectDependencies(this);
        _simSys = _entitySystemManager.GetEntitySystem<SimulationSystem>();
        AlwaysRender = true;
        _renderBuffer = _clyde.CreateBlankTexture<Rgba32>(
            new Vector2i((int) SimulationConfig.SimWidth, (int) SimulationConfig.SimHeight), "simbuffer",
            TextureLoadParameters.Default);
        _bufferClear = new Image<Rgba32>(_renderBuffer.Width, _renderBuffer.Height, new Rgba32(0, 0, 0));
        MouseFilter = MouseFilterMode.Stop;
    }

    public void Update()
    {
        DrawNewFrame();
    }
    
    protected override void KeyBindDown(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.UIClick)
        {
            currentlyDrawing = true;
        }
    }


    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        var pos = args.RelativePosition.RoundedI();
        if (!_simSys.Simulation.SimulationBounds.Contains(pos))
            return;
        MousePosition = pos;
        
        if (currentlyDrawing)
        {
            _simSys.Simulation.Draw(pos, pos + args.Relative.RoundedI(), _simSys.Placing);
        }
    }

    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.UIClick)
        {
            currentlyDrawing = false;
        }
    }
    
    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        var rect = UIBox2
            .FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight) * (UIScale < 1 ? 1.0f : UIScale));
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