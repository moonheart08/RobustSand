using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Wall : ParticleImplementation
{
    public override ParticleType Type => ParticleType.WALL;
    public override string Name => "Wall";
    public override string Description => "An immovable wall.";
    public override byte Weight => 255;
    public override Color Color => Color.Gray;
    public override float RateOfGravity => 0;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.None;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;
}