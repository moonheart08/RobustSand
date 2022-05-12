using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Gas : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    protected override ParticleType PType => ParticleType.Gas;
    protected override string PName => "Gas";

    protected override string PDescription =>
        "Gas? That's like the most generic name ever, at least specify \"flammable\"!";

    protected override byte PWeight => 15;
    protected override Color PColor => Color.GreenYellow;

    protected override float PRateOfGravity => 0.0f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Gas;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Gas;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;

    public override void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        // Burn rapidly.
        sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.Fire);
        var xVec = _random.Next(0, 3) - 1;
        var yVec = _random.Next(0, 3) - 1;
        self.Velocity += new Vector2i(xVec, yVec) * MaximumVelocity;
        
    }
}