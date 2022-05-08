using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Solids;

[Particle]
public sealed class Wood : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    public override ParticleType Type => ParticleType.WOOD;
    public override string Name => "Wood";
    public override string Description => "Actually wooden, unlike the air.";
    public override byte Weight => 255;
    public override Color Color => Color.BurlyWood;
    public override float RateOfGravity => 0;
    public override ParticleMovementFlag MovementFlags => ParticleMovementFlag.None;
    public override ParticlePropertyFlag PropertyFlags => ParticlePropertyFlag.AcidResistant | ParticlePropertyFlag.Solid;

    public override void Burn(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        if (_random.Prob(0.025f))
            sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.FIRE); // I'm on fire! AAAAAAAAAAA
    }
}