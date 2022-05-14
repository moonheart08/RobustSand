using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Sand : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    protected override ParticleType PType => ParticleType.Sand;
    protected override string PName => "Sand";
    protected override string PDescription => "A somewhat heavy dust.";
    protected override byte PWeight => 64;
    protected override Color PColor => Color.SandyBrown;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;
    
    public override void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition,
        Vector2i firePosition, Simulation sim)
    {
        if (_random.Prob(0.90f)) // TODO Replace with heat sim
            return;
        
        sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.Glass);
    }
}