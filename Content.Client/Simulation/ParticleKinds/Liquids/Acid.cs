using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Acid : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override ParticleType PType => ParticleType.ACID;
    protected override string PName => "Acid";
    protected override string PDescription => "An incredibly acidic acid.";
    protected override byte PWeight => 31;
    protected override Color PColor => Color.Magenta;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.AcidResistant;

    public override bool Spawn(ref Particle particle)
    {
        base.Spawn(ref particle);
        particle.Variable1 = 10;
        return true;
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        if (particle.Variable1 <= 0)
        {
            sim.DeleteParticle(id, position, ref particle);
            return;
        }

        for (int relX = -1; relX <= 1; relX++)
        {
            for (int relY = -1; relY <= 1; relY++)
            {
                if (_random.Prob(0.25f))
                    continue;
                var offsPos = position + new Vector2i(relX, relY);
                
                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type == ParticleType.NONE)
                    continue;
                
                if ((sim.Implementations[(int)entry.Type].PropertyFlags & ParticlePropertyFlag.AcidResistant) != 0)
                    continue;
                
                sim.DeleteParticle(entry.Id, offsPos, ref sim.Particles[entry.Id]);
                particle.Variable1 -= 1;
            }
        }
    }
}