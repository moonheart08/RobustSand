using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Acid : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override ParticleType PType => ParticleType.Acid;
    protected override string PName => "Acid";
    protected override string PDescription => "An incredibly acidic acid.";
    protected override byte PWeight => 31;
    protected override Color PColor => Color.Magenta;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Liquid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;

    public override bool OnSpawn(ref Particle particle)
    {
        base.OnSpawn(ref particle);
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

        for (var relX = -1; relX <= 1; relX++)
        {
            for (var relY = -1; relY <= 1; relY++)
            {
                var offsPos = position + new Vector2i(relX, relY);
                
                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type is ParticleType.None or ParticleType.Acid)
                    continue;
                
                if ((sim.Implementations[(int)entry.Type].PropertyFlags & ParticlePropertyFlag.AcidResistant) != 0)
                    continue;
                
                if (_random.Prob(0.25f))
                    continue;
                
                sim.DeleteParticle(entry.Id, offsPos, ref sim.Particles[entry.Id]);
                particle.Variable1 -= 1;
            }
        }
    }
}