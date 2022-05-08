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

    public void Draw(Vector2i pos)
    {
        var entry = Simulation.GetPlayfieldEntry(pos);
        if (Placing == ParticleType.NONE)
        {
            if (entry.Type == ParticleType.NONE)
                return;
            Simulation.DeleteParticle(entry.Id, pos, ref Simulation.Particles[entry.Id]);
        }

        if (entry.Type != ParticleType.NONE)
        {
            Simulation.Implementations[(int)entry.Type].DrawnOn(ref Simulation.Particles[entry.Id], entry.Id, pos, Simulation, Placing);
            return;
        }

        Simulation.TrySpawnParticle(pos, Placing, out _);
    }
}