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

    public Simulation.Simulation Sim;

    public Vector2i MousePosition;

    private bool _currentlyDrawing = false;

    private bool _currentlyErasing = false;

    private OwnedTexture _renderBuffer;

    private OwnedTexture _liquidBuffer;

    private Task? _frameRedrawTask;

    public SimulationControl()
    {
        IoCManager.InjectDependencies(this);
        Sim = _entitySystemManager.GetEntitySystem<SimulationSystem>().Simulation;
        AlwaysRender = true;
        _renderBuffer = _clyde.CreateBlankTexture<Rgba32>(
            new Vector2i((int) SimulationConfig.SimWidth, (int) SimulationConfig.SimHeight), "simbuffer",
            TextureLoadParameters.Default);
        _liquidBuffer = _clyde.CreateBlankTexture<Rgba32>(_renderBuffer.Size, "simbuffer");
        MouseFilter = MouseFilterMode.Stop;
    }

    protected override void KeyBindDown(GUIBoundKeyEventArgs args)
    {
        if (args.Function == EngineKeyFunctions.UIClick)
        {
            _currentlyDrawing = true;
            Sim.DrawBrush(args.RelativePosition.RoundedI(), Sim.Placing);
        } else if (args.Function == EngineKeyFunctions.UIRightClick)
        {
            _currentlyErasing = true;
             Sim.DrawBrush(args.RelativePosition.RoundedI(), ParticleType.None);
        }
    }


    protected override void MouseMove(GUIMouseMoveEventArgs args)
    {
        var pos = args.RelativePosition.RoundedI();
        if (!Sim.SimulationBounds.Contains(pos))
            return;
        MousePosition = pos;

        if (_currentlyDrawing)
        {
            Sim.Draw(pos - args.Relative.RoundedI(), pos, Sim.Placing);
        }

        if (_currentlyErasing)
        {
            Sim.Draw(pos - args.Relative.RoundedI(), pos, ParticleType.None);
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

        if (Sim.RedrawTask?.IsCompleted ?? false)
        {
            _renderBuffer.SetSubImage(Vector2i.Zero, Sim.BaseFrame);
            _liquidBuffer.SetSubImage(Vector2i.Zero, Sim.LiquidFrame);
            Sim.RedrawTask = null;
        }

        var rect = UIBox2
            .FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight) * UIScale);
        
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
        handle.DrawCircle(MousePosition * UIScale, Sim.DrawingRadius, Color.Yellow, false);
        MinSize = UIBox2.FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight)).Size;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        Sim = _entitySystemManager.GetEntitySystem<SimulationSystem>().Simulation;
    }
}