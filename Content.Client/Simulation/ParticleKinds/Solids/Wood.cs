using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Wood : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    protected override ParticleType PType => ParticleType.Wood;
    protected override string PName => "Wood";
    protected override string PDescription => "Actually wooden, unlike the air.";
    protected override byte PWeight => 255;
    protected override Color PColor => Color.BurlyWood;
    protected override float PRateOfGravity => 0;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.None;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Solid | ParticlePropertyFlag.NoTick;

    public override void Burn(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        if (_random.Prob(0.025f))
            sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.Fire); // I'm on fire! AAAAAAAAAAA
    }
}