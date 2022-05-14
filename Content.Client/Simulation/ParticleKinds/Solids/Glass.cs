using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Glass : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Glass;
    protected override string PName => "Glass";
    protected override string PDescription => "Someone burnt their sand.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.LightSkyBlue;
    protected override float PRateOfGravity => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Solid | ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.NoTick;
}