using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Widgets;

[Particle]
public sealed class Spawner : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    public override ParticleType Type => ParticleType.SPAWNER;
    public override string Name => "Spawner";
    public override string Description => "Spawns stuff.";
    public override byte Weight => 255;
    public override Color Color => Color.Yellow;
    public override float RateOfGravity => 0;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.None;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        // Can't spawn particles that don't exist.
        if (particle.Variable1 is <= (int)ParticleType.NONE or >= (int)ParticleType.END)
            return;
        
        for (int relX = -1; relX <=1 ; relX++)
        {
            for (int relY = -1; relY <= 1; relY++)
            {
                if (_random.Prob(0.75f))
                    continue;

                var offsPos = position + new Vector2i(relX, relY);

                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;
                
                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type != ParticleType.NONE)
                    continue;

                sim.TrySpawnParticle(offsPos, (ParticleType) particle.Variable1, out _);
            }
        }
    }

    public override void DrawnOn(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType drawnType)
    {
        if (drawnType is not ParticleType.NONE or ParticleType.SPAWNER)
            particle.Variable1 = (int) drawnType;
    }
}