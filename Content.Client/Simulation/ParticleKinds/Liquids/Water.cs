using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using Serilog;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Water : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;

    protected override ParticleType PType => ParticleType.Water;
    protected override string PName => "Water";
    protected override string PDescription => "It's wet. Probably.";
    protected override byte PWeight => 32;
    protected override Color PColor => Color.FromHex("#0077BE");
    protected override float PRateOfGravity => base.PRateOfGravity * 2;
    protected override float PSpecificHeat => 4.182f;
    protected override (float, ParticleType)? PLowTemperatureConversion => (273.15f, ParticleType.Ice);
    protected override (float, ParticleType)? PHighTemperatureConversion => (373.15f, ParticleType.Steam);
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Liquid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;
    protected override float PBounceCoefficient => 0.3f;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.MovementFlags.HasFlag(ParticleMovementFlag.LiquidAcceleration)  && other.Weight >= Weight ? MovementType.Swap : base.CanMoveThrough(other);
    }

    public override void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        if (fire.Type == ParticleType.Fire)
        {
            // Smother it.
            sim.DeleteParticle(fireId, firePosition, ref fire);
        }
    }
}