using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Sand : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.SAND;
    protected override string PName => "Sand";
    protected override string PDescription => "A somewhat heavy dust.";
    protected override byte PWeight => 64;
    protected override Color PColor => Color.SandyBrown;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
}