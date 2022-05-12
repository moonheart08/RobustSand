using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Oil : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Oil;
    protected override string PName => "Oil";
    protected override string PDescription => "Careful, you might summon the US Military.";
    protected override byte PWeight => 24;
    protected override float PRateOfGravity => base.PRateOfGravity * 1.8f;
    protected override Color PColor => Color.FromHex("#DBCF5C");
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Liquid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;
}