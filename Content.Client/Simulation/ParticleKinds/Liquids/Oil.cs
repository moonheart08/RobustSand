using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Oil : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    protected override ParticleType PType => ParticleType.Oil;
    protected override string PName => "Oil";
    protected override string PDescription => "Careful, you might summon the US Military.";
    protected override byte PWeight => 24;
    protected override float PRateOfGravity => base.PRateOfGravity * 1.8f;
    protected override Color PColor => Color.FromHex("#DBCF5C");
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Liquid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;

    public override void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition,
        Vector2i firePosition, Simulation sim)
    {
        if (_random.Prob(0.99f))
            return;
        
        sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.Fire);
        var xVec = _random.Next(0, 3) - 1;
        var yVec = _random.Next(0, 3) - 1;
        self.Velocity += new Vector2i(xVec, yVec) * MaximumVelocity;
    }
}