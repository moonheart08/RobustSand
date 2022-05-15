using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Steam : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Steam;
    protected override string PName => "Steam";
    protected override string PDescription => "Not the thing you're playing this from, if you even are.";
    protected override byte PWeight => 13;
    protected override Color PColor => Color.WhiteSmoke;
    
    protected override float PRateOfGravity => -base.PRateOfGravity / 2;

    protected override float PMaximumVelocity => 2.5f;
    protected override float PSpecificHeat => 2.020f;

    protected override float PDiffusionRate => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Gas;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Gas;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;
}