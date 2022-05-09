using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

[Particle]
public sealed class None : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.NONE;
    protected override string PName => "None";
    protected override string PDescription => "Doesn't exist.";
    protected override byte PWeight => 0;
    protected override Color PColor => Color.SandyBrown;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
}