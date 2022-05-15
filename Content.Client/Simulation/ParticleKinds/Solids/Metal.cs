using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Metal : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Metal;
    protected override string PName => "Metal";
    protected override string PDescription => "Eris branded.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.FromHex("#556a84");
    protected override float PRateOfGravity => 0;
    protected override float PSpecificHeat => .209f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Solid;

}