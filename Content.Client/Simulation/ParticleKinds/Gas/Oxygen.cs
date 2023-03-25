using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Oxygen : ParticleImplementation
{
    protected override ParticleType PType => ParticleType.Oxygen;
    protected override string PName => "Oxygen";
    protected override string PDescription => "Now no longer breathable!";
    protected override byte PWeight => 15;
    protected override Color PColor => Color.FromHex("#92b6d5");

    protected override float PRateOfGravity => 0.0f;
    protected override float PSpecificHeat => 2.85f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Gas;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Gas;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;

    private const float TemperatureUp = 515.0f;
    
    public override void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        // Burn with great heat.
        sim.ChangeParticleType(selfId, selfPosition, ref self, ParticleType.Fire);
        self.Temperature = fire.Temperature;

        fire.Temperature += TemperatureUp;
    }
}