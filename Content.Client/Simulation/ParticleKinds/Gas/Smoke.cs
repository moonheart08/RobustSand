using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Smoke : ParticleImplementation
{
    public override ParticleType Type => ParticleType.SMOKE;
    public override string Name => "Smoke";
    public override string Description => "This looks like a job for Woodsy Owl.";
    public override byte Weight => 14;
    public override Color Color => Color.WhiteSmoke;
    public override float RateOfGravity => -base.RateOfGravity / 2;
    public override float MaximumVelocity => 2f;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.None;

    public const int SmokeLifespan = 30 * 15;

    public override bool Spawn(ref Particle particle)
    {
        particle.Variable1 = SmokeLifespan;
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
            sim.DeleteParticle(id, position, ref particle);
            return;
        }

        particle.Variable1--;
    }
}