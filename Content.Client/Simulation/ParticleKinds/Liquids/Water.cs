using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Water : ParticleImplementation
{
    public override ParticleType Type => ParticleType.WATER;
    public override string Name => "Water";
    public override string Description => "It's wet. Probably.";
    public override byte Weight => 32;
    public override Color Color => Color.MediumAquamarine;
    public override float RateOfGravity => base.RateOfGravity * 2;
    public override ParticleMovementProperty MovementProperties => ParticleMovementProperty.Liquid;
}