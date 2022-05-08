using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

[Particle]
public sealed class None : ParticleImplementation
{
    public override ParticleType Type => ParticleType.NONE;
    public override string Name => "None";
    public override string Description => "Doesn't exist.";
    public override byte Weight => 0;
    public override Color Color => Color.SandyBrown;
    public override ParticleMovementProperty MovementProperties => ParticleMovementProperty.None;
}