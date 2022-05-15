using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Widgets;

[Particle]
public sealed class Void : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Void;
    protected override string PName => "Void";
    protected override string PDescription => "A bottomless void to delete your infinite sand.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.Maroon;
    protected override float PRateOfGravity => 0.0f;
    protected override float PSpecificHeat => 0.0f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.Type == ParticleType.Void ? MovementType.Block : MovementType.Custom;
    }

    public override bool OnMovedInto(ref Particle self, ref Particle other, uint selfId, uint otherId, Vector2i selfPosition,
        Vector2i otherPosition, Simulation sim)
    {
        sim.DeleteParticle(otherId, otherPosition, ref other);
        return true;
    }
}