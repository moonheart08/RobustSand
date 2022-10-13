using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Ice : MoltenParticle
{
    protected override ParticleType PType => ParticleType.Ice;
    protected override string PName => "Ice";
    protected override string PDescription => "It's anti-wet.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.FromHex("#c9dfe3");
    protected override float PRateOfGravity => 0.0f;
    protected override float PSpecificHeat => 4.182f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Solid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Basic;
    public override bool MeltOrFreeze => false; // We're the frozen form of stuff.
    public override float DefaultChangePoint => 273.15f;
    public override ParticleType FailsafeType => ParticleType.Water;
    public override string Adjective => "Frozen";

    public override bool OnSpawn(ref Particle particle)
    {
        particle.Temperature = 263.15f; // frozen.
        return true;
    }
}