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
    
    public override ParticleType Type => ParticleType.WATER;
    public override string Name => "Water";
    public override string Description => "It's wet. Probably.";
    public override byte Weight => 32;
    public override Color Color => Color.MediumAquamarine;
    public override float RateOfGravity => base.RateOfGravity * 2;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.Liquid | ParticleMovementFlag.Spread;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.None;

    public override float BounceCoefficient => 0.3f;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.MovementFlags.HasFlag(ParticleMovementFlag.Liquid) ? MovementType.Swap : base.CanMoveThrough(other);
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
    }
}