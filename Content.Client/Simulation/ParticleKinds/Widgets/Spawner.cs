using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Widgets;

[Particle]
public sealed class Spawner : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override ParticleType PType => ParticleType.Spawner;
    protected override string PName => "Spawner";
    protected override string PDescription => "Spawns stuff.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.Yellow;
    protected override float PRateOfGravity => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        // Can't spawn particles that don't exist.
        if (particle.Variable1 is <= (int)ParticleType.None or >= (int)ParticleType.End)
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
                
                if (entry.Type != ParticleType.None)
                    continue;

                sim.TrySpawnParticle(offsPos, (ParticleType) particle.Variable1, out _);
            }
        }
    }

    public override void DrawnOn(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType drawnType)
    {
        if (drawnType is not ParticleType.None and not ParticleType.Spawner)
            particle.Variable1 = (int) drawnType;
    }
}