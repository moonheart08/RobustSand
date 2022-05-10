using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Coal : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    protected override ParticleType PType => ParticleType.COAL;
    protected override string PName => "Coal";

    protected override string PDescription =>
        "The worst ore, unless you're playing 1.18 in which case you never have enough.";

    protected override byte PWeight => 128;
    protected override Color PColor => Color.FromHex("#FFFFFF");
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Spread;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.None;

    public override bool Spawn(ref Particle particle)
    {
        particle.Variable1 = 480;
        return true;
    }

    public override void Burn(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition, Vector2i firePosition,
        Simulation sim)
    {
        self.Variable2 = 1;
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        if (particle.Variable2 <= 0)
            return; // not on fire, do nothing.

        if (particle.Variable1 <= 0)
        {
            sim.ChangeParticleType(id, position, ref particle, ParticleType.FIRE);
            return;
        }

        for (int relX = -1; relX <= 1; relX++)
        {
            for (int relY = -1; relY <= 1; relY++)
            {
                if (_random.Prob(0.95f))
                    continue;
                var offsPos = position + new Vector2i(relX, relY);

                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);

                if (entry.Type == ParticleType.WATER)
                    particle.Variable2 = 0; // Put out the flames.
                
                if (entry.Type != ParticleType.NONE)
                    continue;

                sim.TrySpawnParticle(offsPos, ParticleType.FIRE, out _);
            }
        }

        particle.Variable1--;

    }
}