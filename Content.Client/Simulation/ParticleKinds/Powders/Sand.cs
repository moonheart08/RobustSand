using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Sand : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Sand;
    protected override string PName => "Sand";
    protected override string PDescription => "A somewhat heavy dust.";
    protected override byte PWeight => 64;
    protected override Color PColor => Color.SandyBrown;
    protected override float PSpecificHeat => .840f;
    protected override (float, ParticleType)? PHighTemperatureConversion => (1973.15f, ParticleType.Glass);
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
}