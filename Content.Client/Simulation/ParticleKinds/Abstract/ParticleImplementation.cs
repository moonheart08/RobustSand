using System;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

public abstract class ParticleImplementation
{
    /// <summary>
    /// The type of the particle. No duplicates, new particles need a new type, one implementation per type.
    /// </summary>
    public abstract ParticleType Type { get; }
    
    /// <summary>
    /// The name of the particle.
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// The description of the particle.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Controls whether a particle sinks or not through another particle.
    /// Higher values sink through lower values by default.
    /// </summary>
    public abstract byte Weight { get; }

    /// <summary>
    /// The color of the particle.
    /// </summary>
    public abstract Color Color { get; }

    /// <summary>
    /// The rate of gravitational pull (to the floor) for the particle.
    /// </summary>
    public virtual float RateOfGravity { get; } = 0.05f;

    public virtual float BounceCoefficient { get; } = 0.1f;

    public virtual float MaximumVelocity { get; } = 4.0f;

    public abstract ParticleMovementFlag MovementFlags { get; }
    
    public abstract ParticlePropertyFlag PropertyFlags { get; }

    /// <summary>
    /// Whether or not this particle can move through the given other particle.
    /// This information is cached.
    /// </summary>
    /// <param name="other">The particle to check against.</param>
    /// <returns>Whether or not this particle can move through the other.</returns>
    public virtual MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.Weight > Weight ? MovementType.Swap : MovementType.Block;
    }
    
    public virtual void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        // Do nothing.
    }

    public virtual void Render(ref Particle particle, out Color color)
    {
        color = Color; // do the minimum amount of work.
    }

    public virtual bool Spawn(ref Particle particle)
    {
        return true;
    }

    public virtual void Delete(ref Particle particle)
    {
        
    }

    public virtual void DrawnOn(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType drawnType)
    {
        
    }

    public virtual void ChangedType(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType oldType)
    {
        
    }

    public virtual void Burn(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition,
        Vector2i firePosition, Simulation sim)
    {
        
    }
    
    public virtual bool DoMovement(ref Particle self, ref Particle other, uint selfId, uint otherId,
        Vector2i selfPosition, Vector2i otherPosition, Simulation sim)
    {
        throw new NotImplementedException();
    }
}

[Flags]
public enum ParticleMovementFlag
{
    None = 0,
    Spread = 1,
    Liquid = 2,
}

[Flags]
public enum ParticlePropertyFlag
{
    None = 0,
    AcidResistant = 1,
    Solid = 2,
}