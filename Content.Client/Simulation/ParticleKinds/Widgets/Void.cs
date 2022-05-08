using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Widgets;

[Particle]
public sealed class Void : ParticleImplementation
{
    public override ParticleType Type => ParticleType.VOID;
    public override string Name => "Void";
    public override string Description => "A bottomless void to delete your infinite sand.";
    public override byte Weight => 255;
    public override Color Color => Color.Maroon;
    public override float RateOfGravity => 0.0f;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.None;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.Type == ParticleType.VOID ? MovementType.Block : MovementType.Custom;
    }

    public override bool DoMovement(ref Particle self, ref Particle other, uint selfId, uint otherId, Vector2i selfPosition,
        Vector2i otherPosition, Simulation sim)
    {
        sim.DeleteParticle(otherId, otherPosition, ref other);
        return true;
    }
}