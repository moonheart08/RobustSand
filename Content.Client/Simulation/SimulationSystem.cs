using System;
using System.Threading.Tasks;
using Content.Client.Input;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.Maths;
using Robust.Shared.Players;
using Robust.Shared.Timing;

namespace Content.Client.Simulation;

public sealed class SimulationSystem : EntitySystem
{
    [Dependency] private readonly IInputManager _inputManager = default!;
    
    public ParticleType Placing = ParticleType.Sand;
    
    public Simulation Simulation;
    private Stopwatch SimStopWatch = new Stopwatch();
    public TimeSpan SimTickTime = TimeSpan.Zero;

    public SimulationSystem()
    {
        Simulation = new Simulation();
    }
    
    public void UpdateA(float frameTime)
    {
        SimStopWatch.Restart();
        Simulation.RunFrame();

        SimTickTime = SimStopWatch.Elapsed;
    }

    public override void Initialize()
    {
        _inputManager.SetInputCommand(ContentKeyFunctions.BrushSizeUp, InputCmdHandler.FromDelegate(BrushSizeUp));
        _inputManager.SetInputCommand(ContentKeyFunctions.BrushSizeDown, InputCmdHandler.FromDelegate(BrushSizeDown));
    }

    private void BrushSizeUp(ICommonSession? session)
    {
        Simulation.DrawingRadius += 1;
    }
    
    private void BrushSizeDown(ICommonSession? session)
    {
        if (Simulation.DrawingRadius > 1)
            Simulation.DrawingRadius -= 1;
    }
}