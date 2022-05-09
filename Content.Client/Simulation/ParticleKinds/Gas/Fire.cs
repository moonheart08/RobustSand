using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Fire : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.FIRE;
    protected override string PName => "Fire";
    protected override string PDescription => "It burns, it burns!";
    protected override byte PWeight => 15;
    protected override Color PColor => Color.Firebrick;
    protected override float PRateOfGravity => -base.PRateOfGravity / 3;
    protected override float PDiffusionRate => 0.05f;
    protected override float PMaximumVelocity => 1.4f;
    // TODO: if/when airsim is coded make this a gas.
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;

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
        
        for (var relX = -1; relX <= 1; relX++)
        {
            for (var relY = -1; relY <= 1; relY++)
            {
                var offsPos = position + new Vector2i(relX, relY);
                
                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type == ParticleType.NONE)
                    continue;

                sim.Implementations[(int) entry.Type].Burn(ref sim.Particles[entry.Id], ref particle, entry.Id, id,
                    offsPos, position, sim);
                
                if (particle.Type != ParticleType.FIRE)
                    return; // We can't burn things if we're not fire. May also have been deleted.
            }
        }
    }
}