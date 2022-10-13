using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Stone : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Stone;
    protected override string PName => "Stone";
    protected override string PDescription => "For hitting people with.";
    protected override byte PWeight => 90;
    protected override Color PColor => Color.DarkGray;
    protected override float PSpecificHeat => .840f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;

}