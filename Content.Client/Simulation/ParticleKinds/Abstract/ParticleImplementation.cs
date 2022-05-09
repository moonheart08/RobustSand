using System;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

public abstract class ParticleImplementation
{
    /// <summary>
    /// The type of the particle. No duplicates, new particles need a new type, one implementation per type.
    /// </summary>
    protected abstract ParticleType PType { get; }

    public ParticleType Type;
    
    /// <summary>
    /// The name of the particle.
    /// </summary>
    protected abstract string PName { get; }

    public string Name;
    
    /// <summary>
    /// The description of the particle.
    /// </summary>
    protected abstract string PDescription { get; }
    
    public string Description;

    /// <summary>
    /// Controls whether a particle sinks or not through another particle.
    /// Higher values sink through lower values by default.
    /// </summary>
    protected abstract byte PWeight { get; }

    public byte Weight;

    /// <summary>
    /// The color of the particle.
    /// </summary>
    protected abstract Color PColor { get; }

    public Color Color;

    /// <summary>
    /// The rate of gravitational pull (to the floor) for the particle.
    /// </summary>
    protected virtual float PRateOfGravity { get; } = 0.05f;

    public float RateOfGravity;

    protected virtual float PBounceCoefficient { get; } = 0.1f;

    public float BounceCoefficient;

    protected virtual float PMaximumVelocity { get; } = 4.0f;

    public float MaximumVelocity;

    protected abstract ParticleMovementFlag PMovementFlags { get; }

    public ParticleMovementFlag MovementFlags;

    protected abstract ParticlePropertyFlag PPropertyFlags { get; }

    public ParticlePropertyFlag PropertyFlags;


    protected ParticleImplementation()
    {
        Type = PType;
        Name = PName;
        Description = PDescription;
        Weight = PWeight;
        Color = PColor;
        RateOfGravity = PRateOfGravity;
        BounceCoefficient = PBounceCoefficient;
        MaximumVelocity = PMaximumVelocity;
        MovementFlags = PMovementFlags;
        PropertyFlags = PPropertyFlags;
    }

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