using System;
using System.Threading.Tasks;
using Content.Client.Input;
using Robust.Client.UserInterface;
using Content.Client.Simulation;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Robust.Shared.Maths.Color;

namespace Content.Client.Rendering;

public sealed partial class SimulationControl : Control
{
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;

    public float UpdateRate = 1.0f / 30;

    public float Accumulator = 0;

    public SimulationSystem _simSys;

    public Vector2i MousePosition;

    private bool _currentlyDrawing = false;

    private bool _currentlyErasing = false;

    private OwnedTexture _renderBuffer;

    private OwnedTexture _liquidBuffer;

    public SimulationControl()
    {
        IoCManager.InjectDependencies(this);
        _simSys = _entitySystemManager.GetEntitySystem<SimulationSystem>();
        AlwaysRender = true;
        _renderBuffer = _clyde.CreateBlankTexture<Rgba32>(
            new Vector2i((int) SimulationConfig.SimWidth, (int) SimulationConfig.SimHeight), "simbuffer",
            TextureLoadParameters.Default);
        _liquidBuffer = _clyde.CreateBlankTexture<Rgba32>(_renderBuffer.Size, "simbuffer");
        _bufferClear = new Image<Rgba32>(_renderBuffer.Width, _renderBuffer.Height, new Rgba32(0, 0, 0, 0));
        _newFrame = _bufferClear.Clone();
        _liquidFrame = _bufferClear.Clone();
        _bufferFrameBox = UIBox2i.FromDimensions(Vector2i.Zero, _renderBuffer.Size);
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
            _currentlyDrawing = true;
        } else if (args.Function == EngineKeyFunctions.UIRightClick)
        {
            _currentlyErasing = true;
        }
    }


    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        var pos = args.RelativePosition.RoundedI();
        if (!_simSys.Simulation.SimulationBounds.Contains(pos))
            return;
        MousePosition = pos;

        if (_currentlyDrawing)
        {
            _simSys.Simulation.Draw(pos, pos + args.Relative.RoundedI(), _simSys.Placing);
        }

        if (_currentlyErasing)
        {
            _simSys.Simulation.Draw(pos, pos + args.Relative.RoundedI(), ParticleType.NONE);
        }
    }

    protected override void KeyBindUp(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.UIClick)
        {
            _currentlyDrawing = false;
        } else if (args.Function == EngineKeyFunctions.UIRightClick)
        {
            _currentlyErasing = false;
        }
    }
    
    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        var rect = UIBox2
            .FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight) * (UIScale < 1 ? 1.0f : UIScale));
        
        handle.DrawTextureRect(_renderBuffer, rect);
        for (var relX = -2; relX < 3; relX++)
        {
            for (var relY = -2; relY < 3; relY++)
            {
                var amt = (float)(Math.Exp(-0.3 * (relX * relX + relY * relY)) / 5.0);

                handle.DrawTextureRect(_liquidBuffer, rect.Translated(new Vector2(relX, relY)), new Color(1.0f, 1.0f, 1.0f, amt));
            }
        }
        // Fire effects use point primitives.
        //handle.DrawPrimitives(DrawPrimitiveTopology.PointList, );
        handle.DrawCircle(MousePosition, _simSys.Simulation.DrawingRadius, Color.Yellow, false);
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