using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Simulation;

public sealed class SimulationSystem : EntitySystem
{
    public bool SimPaused = false;
    public ParticleType Placing = ParticleType.SAND;
    public readonly Simulation Simulation = new Simulation();
    private Stopwatch SimStopWatch = new Stopwatch();
    public TimeSpan SimTickTime = TimeSpan.Zero;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        SimStopWatch.Restart();
        if (!SimPaused)
            Simulation.RunFrame();
        SimTickTime = SimStopWatch.Elapsed;
    }
}