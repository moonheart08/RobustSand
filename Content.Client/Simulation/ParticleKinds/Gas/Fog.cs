using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Fog : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Fog;
    protected override string PName => "Fog";
    protected override string PDescription => "On one foggy evening...";
    protected override byte PWeight => 31;
    protected override Color PColor => Color.WhiteSmoke;
    
    protected override float PRateOfGravity => 0.02f;

    protected override float PMaximumVelocity => 0.125f;

    protected override float PDiffusionRate => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Gas;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Gas;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;
}