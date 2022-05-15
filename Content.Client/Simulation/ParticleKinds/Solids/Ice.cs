using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Ice : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Ice;
    protected override string PName => "Ice";
    protected override string PDescription => "It's anti-wet.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.FromHex("#c9dfe3");
    protected override float PRateOfGravity => 0.0f;
    protected override float PSpecificHeat => 4.182f;
    protected override (float, ParticleType)? PHighTemperatureConversion => (273.15f, ParticleType.Water);
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Solid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Basic;

    public override bool OnSpawn(ref Particle particle)
    {
        particle.Temperature = 263.15f; // frozen.
        return true;
    }
}