using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Simulation;

public sealed class SimulationSystem : EntitySystem
{
    public bool SimPaused = false;
    public ParticleType Placing = ParticleType.SAND;
    public readonly Simulation Simulation = new Simulation();

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (!SimPaused)
            Simulation.RunFrame();
    }
}