using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation;

/// <summary>
/// The particle data structure.
/// </summary>
public struct Particle
{
    public Vector2 Position;
    public Vector2 Velocity;
    public ParticleType Type = ParticleType.NONE;
    public int Variable1;
    public int Variable2;

    /// <summary>
    /// Constructs a particle
    /// </summary>
    /// <param name="position">Position to construct it at.</param>
    public Particle(Vector2 position, ParticleType type) : this()
    {
        Position = position;
        Type = type;
        Velocity = Vector2.Zero;
        Variable1 = 0;
        Variable2 = 0;
    }
}