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

    protected override ParticleType PType => ParticleType.WATER;
    protected override string PName => "Water";
    protected override string PDescription => "It's wet. Probably.";
    protected override byte PWeight => 32;
    protected override Color PColor => Color.MediumAquamarine;
    protected override float PRateOfGravity => base.PRateOfGravity * 2;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
    protected override float PBounceCoefficient => 0.3f;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.MovementFlags.HasFlag(ParticleMovementFlag.Liquid) ? MovementType.Swap : base.CanMoveThrough(other);
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
    }
}