using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Liquids;

[Particle]
public sealed class Lava : MoltenParticle
{
    protected override ParticleType PType => ParticleType.Lava;
    protected override string PName => "Lava";
    protected override string PDescription => "For when you want to melt stuff.";
    protected override byte PWeight => 32;
    protected override Color PColor => Color.FromHex("#0077BE");
    protected override float PRateOfGravity => base.PRateOfGravity * 4;
    protected override float PSpecificHeat => 1.01f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Liquid;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;
    protected override float PBounceCoefficient => 0.3f;
    public override bool MeltOrFreeze => true; // We're the molten form of stuff.
    public override float DefaultChangePoint => 400.0f;
    public override ParticleType FailsafeType => ParticleType.Stone;
    public override string Adjective => "Molten";
    
    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.MovementFlags.HasFlag(ParticleMovementFlag.LiquidAcceleration)  && other.Weight >= Weight ? MovementType.Swap : base.CanMoveThrough(other);
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        for (var relX = -1; relX <= 1; relX++)
        {
            for (var relY = -1; relY <= 1; relY++)
            {
                var offsPos = position + new Vector2i(relX, relY);

                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);

                if (entry.Type == ParticleType.None)
                    continue;

                sim.Implementations[(int) entry.Type].OnBurned(ref sim.Particles[entry.Id], ref particle, entry.Id, id,
                    offsPos, position, sim);

                if (particle.Type != Type)
                    return; // We can't burn things if we're not the correct type.
            }
        }

        base.Update(ref particle, id, position, sim);
    }
}