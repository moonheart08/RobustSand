using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Smoke : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.SMOKE;
    protected override string PName => "Smoke";
    protected override string PDescription => "This looks like a job for Woodsy Owl.";
    protected override byte PWeight => 14;
    protected override Color PColor => Color.WhiteSmoke;
    protected override float PRateOfGravity => -base.PRateOfGravity / 2;
    protected override float PMaximumVelocity => 2f;
    // TODO: if/when airsim is coded make this a gas.
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.None | ParticleRenderFlag.Blob;

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

    public override void Render(ref Particle particle, out Color color)
    {
        base.Render(ref particle, out var oldColor);
        color = oldColor with {A = Math.Clamp(SmokeLifespan - particle.Variable1, 0, 64) / 64.0f };
    }
}