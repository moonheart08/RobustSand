using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Wall : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Wall;
    protected override string PName => "Wall";
    protected override string PDescription => "An immovable wall.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.Gray;
    protected override float PRateOfGravity => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;
}