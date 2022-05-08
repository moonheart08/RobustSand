using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Fire : ParticleImplementation
{

    public override ParticleType Type => ParticleType.FIRE;
    public override string Name => "Fire";
    public override string Description => "It burns, it burns!";
    public override byte Weight => 15;
    public override Color Color => Color.Firebrick;
    public override float RateOfGravity => -base.RateOfGravity / 3;
    public override float MaximumVelocity => 1.4f;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.None;

    public const int FireLifespan = 30 * 5;

    public override bool Spawn(ref Particle particle)
    {
        particle.Variable1 = FireLifespan;
        return true;
    }
    
    public override void ChangedType(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType oldType)
    {
        Spawn(ref particle); // Do the usual setup.
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        if (particle.Variable1 <= 0)
        {
            sim.ChangeParticleType(id, position, ref particle, ParticleType.SMOKE);
            return;
        }

        particle.Variable1--;
        
        for (int relX = -1; relX <= 1; relX++)
        {
            for (int relY = -1; relY <= 1; relY++)
            {
                var offsPos = position + new Vector2i(relX, relY);
                
                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type == ParticleType.NONE)
                    continue;

                sim.Implementations[(int) entry.Type].Burn(ref sim.Particles[entry.Id], ref particle, entry.Id, id,
                    offsPos, position, sim);
            }
        }
    }
}