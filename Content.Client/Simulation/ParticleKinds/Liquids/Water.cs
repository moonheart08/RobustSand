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
    public override ParticleMovementProperty MovementProperties => ParticleMovementProperty.Liquid | ParticleMovementProperty.Spread;

    public override MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.MovementProperties.HasFlag(ParticleMovementProperty.Liquid) ? MovementType.Swap : base.CanMoveThrough(other);
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        var pos = position + new Vector2i(0, 1);
        if (!sim.SimulationBounds.Contains(pos))
            return;

        var entry = sim.GetPlayfieldEntry(pos);
        if (entry.Type != ParticleType.NONE)
        {

            var whichFirst = _random.Prob(0.5f);
            var success = false;
            
            for (var i = 0; i < 2 && !success && particle.Type != ParticleType.NONE; i++)
            {
                if (whichFirst)
                {
                    success = sim.TryMoveParticle(id, position + Vector2.UnitX, ref particle);
                }
                else
                {
                    success = sim.TryMoveParticle(id, position - Vector2.UnitX, ref particle);
                }

                whichFirst = !whichFirst;
            }
        }
    }
}