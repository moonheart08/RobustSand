using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Powders;

[Particle]
public sealed class Sand : ParticleImplementation
{
    public override ParticleType Type => ParticleType.SAND;
    public override string Name => "Sand";
    public override string Description => "A somewhat heavy dust.";
    public override byte Weight => 64;
    public override Color Color => Color.SandyBrown;
    public override ParticleMovementProperty MovementProperties => ParticleMovementProperty.Spread;
}