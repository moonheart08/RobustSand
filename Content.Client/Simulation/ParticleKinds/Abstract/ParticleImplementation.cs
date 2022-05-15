using System;
using Robust.Shared.Maths;
using Robust.Shared.ViewVariables;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

public abstract class ParticleImplementation
{
    /// <summary>
    /// The type of the particle. No duplicates, new particles need a new type, one implementation per type.
    /// </summary>
    protected abstract ParticleType PType { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly ParticleType Type;
    
    /// <summary>
    /// The name of the particle.
    /// </summary>
    protected abstract string PName { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly string Name;
    
    /// <summary>
    /// The description of the particle.
    /// </summary>
    protected abstract string PDescription { get; }
    
    [ViewVariables(VVAccess.ReadWrite)]
    public readonly string Description;

    /// <summary>
    /// Controls whether a particle sinks or not through another particle.
    /// Higher values sink through lower values by default.
    /// </summary>
    protected abstract byte PWeight { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly byte Weight;

    /// <summary>
    /// The color of the particle.
    /// </summary>
    protected abstract Color PColor { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly Color Color;

    /// <summary>
    /// The rate of gravitational pull (to the floor) for the particle.
    /// </summary>
    protected virtual float PRateOfGravity { get; } = 0.05f;

    public readonly float RateOfGravity;

    /// <summary>
    /// The rate of diffusion for gaseous particles.
    /// </summary>
    protected virtual float PDiffusionRate => PRateOfGravity;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly float DiffusionRate;

    /// <summary>
    /// How much energy this particle should keep when bouncing off another particle.
    /// </summary>
    protected virtual float PBounceCoefficient { get; } = 0.1f;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly float BounceCoefficient;

    /// <summary>
    /// The maximum velocity (pixles per tick) this particle is allowed to has. 
    /// </summary>
    protected virtual float PMaximumVelocity { get; } = 4.0f;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly float MaximumVelocity;

    /// <summary>
    /// The specific heat of the particle. Stored in kJ/(kg K)
    /// </summary>
    protected abstract float PSpecificHeat { get; }
    
    [ViewVariables(VVAccess.ReadWrite)]
    public readonly float SpecificHeat;

    protected virtual (float temperature, ParticleType type)? PHighTemperatureConversion { get; } = null;

    public readonly (float temperature, ParticleType type)? HighTemperatureConversion;
    
    protected virtual (float temperature, ParticleType type)? PLowTemperatureConversion { get; } = null;
    
    public readonly (float temperature, ParticleType type)? LowTemperatureConversion;
    
    /// <summary>
    /// Movement flags for a particle, controlling how it moves.
    /// </summary>
    protected abstract ParticleMovementFlag PMovementFlags { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly ParticleMovementFlag MovementFlags;

    /// <summary>
    /// Property flags for a particle. Controls things like acid resistance, or what type of matter the particle is (solid, liquid, gas)
    /// </summary>
    protected abstract ParticlePropertyFlag PPropertyFlags { get; }

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly ParticlePropertyFlag PropertyFlags;

    /// <summary>
    /// Rendering flags for a particle. Dictates which layers it's drawn to.
    /// </summary>
    protected virtual ParticleRenderFlag PParticleRenderFlags { get; } = ParticleRenderFlag.Basic;

    [ViewVariables(VVAccess.ReadWrite)]
    public readonly ParticleRenderFlag ParticleRenderFlags;

    protected ParticleImplementation()
    {
        // Shoo, this is intended and it works as intended resharper.
        // ReSharper disable function VirtualMemberCallInConstructor
        Type = PType;
        Name = PName;
        Description = PDescription;
        Weight = PWeight;
        Color = PColor;
        RateOfGravity = PRateOfGravity;
        BounceCoefficient = PBounceCoefficient;
        MaximumVelocity = PMaximumVelocity;
        SpecificHeat = PSpecificHeat;
        HighTemperatureConversion = PHighTemperatureConversion;
        LowTemperatureConversion = PLowTemperatureConversion;
        MovementFlags = PMovementFlags;
        PropertyFlags = PPropertyFlags;
        DiffusionRate = PDiffusionRate;
        ParticleRenderFlags = PParticleRenderFlags;
    }

    /// <summary>
    /// Whether or not this particle can move through the given other particle.
    /// This information is calculated and cached on simulation setup.
    /// </summary>
    /// <param name="other">The particle to check against.</param>
    /// <returns>Whether or not this particle can move through the other.</returns>
    public virtual MovementType CanMoveThrough(ParticleImplementation other)
    {
        return other.Weight > Weight ? MovementType.Swap : MovementType.Block;
    }
    
    /// <summary>
    /// The update function for a particle. Should handle anything it needs to do every tick.
    /// </summary>
    /// <param name="particle">Reference to the current particle.</param>
    /// <param name="id">Current particle's ID.</param>
    /// <param name="position">Current particle's position.</param>
    /// <param name="sim">The simulation this is running in.</param>
    public virtual void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        // Do nothing.
    }

    /// <summary>
    /// The render function for a particle. Should spit out a color to use.
    /// </summary>
    /// <param name="particle">Reference to the current particle.</param>
    /// <param name="color">Output color.</param>
    public virtual void Render(ref Particle particle, out Color color)
    {
        color = Color; // do the minimum amount of work.
    }

    /// <summary>
    /// The function ran when a particle of this type is spawned.
    /// </summary>
    /// <param name="particle">Reference to the spawned particle.</param>
    /// <returns>Whether or not spawning should succeed.</returns>
    public virtual bool OnSpawn(ref Particle particle)
    {
        return true;
    }

    /// <summary>
    /// The function ran when a particle of this type is deleted.
    /// </summary>
    /// <param name="particle">Reference to the deleted particle.</param>
    public virtual void OnDelete(ref Particle particle)
    {
        
    }

    /// <summary>
    /// Function ran when the user uses the brush to draw over this particle.
    /// </summary>
    /// <param name="particle">Reference to the current particle.</param>
    /// <param name="id">Current particle's ID.</param>
    /// <param name="position">Current particle's position.</param>
    /// <param name="sim">The simulation this is running in.</param>
    /// <param name="drawnType">The particle type drawn over with.</param>
    public virtual void OnDrawnOver(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType drawnType)
    {
        
    }

    /// <summary>
    /// Function ran when a particle of changes type to this type.
    /// </summary>
    /// <param name="particle">Reference to the current particle.</param>
    /// <param name="id">Current particle's ID.</param>
    /// <param name="position">Current particle's position.</param>
    /// <param name="sim">The simulation this is running in.</param>
    /// <param name="oldType">The original particle type.</param>
    public virtual void OnChangedIntoType(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType oldType)
    {
        
    }

    /// <summary>
    /// Function ran when fire burns a particle of this type.
    /// </summary>
    /// <param name="self">Reference to the current particle.</param>
    /// <param name="fire">Reference to the burning particle.></param>
    /// <param name="selfId">Current particle's ID.</param>
    /// <param name="fireId">Burning particle's ID.</param>
    /// <param name="selfPosition">Current particle's position.</param>
    /// <param name="firePosition">Burning particle's position.</param>
    /// <param name="sim">The simulation this is running in.</param>
    public virtual void OnBurned(ref Particle self, ref Particle fire, uint selfId, uint fireId, Vector2i selfPosition,
        Vector2i firePosition, Simulation sim)
    {
        
    }
    
    /// <summary>
    /// Function ran when another particle moves into this one, and the set behavior is MovementType.Custom.
    /// </summary>
    /// <param name="self">Reference to the current particle.</param>
    /// <param name="other">Reference to the colliding particle.></param>
    /// <param name="selfId">Current particle's ID.</param>
    /// <param name="otherId">Colliding particle's ID.</param>
    /// <param name="selfPosition">Current particle's position.</param>
    /// <param name="otherPosition">Colliding particle's position.</param>
    /// <param name="sim">The simulation this is running in.</param>
    /// <returns>Whether or not the movement should be considered to have exceeded.</returns>
    /// <exception cref="NotImplementedException">Thrown if you have CanMoveThrough return Custom without implementing this function.</exception>
    public virtual bool OnMovedInto(ref Particle self, ref Particle other, uint selfId, uint otherId,
        Vector2i selfPosition, Vector2i otherPosition, Simulation sim)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Flags that control particle movement behavior.
/// </summary>
[Flags]
public enum ParticleMovementFlag : uint
{
    None = 0,
    /// <summary>
    /// Indicates this particle should try and fall to the sides, like a dust.
    /// </summary>
    Spread = 1,
    /// <summary>
    /// This particle should have liquid acceleration.
    /// </summary>
    LiquidAcceleration = 2,
    /// <summary>
    /// This particle should behave like a liquid.
    /// </summary>
    Liquid = Spread | LiquidAcceleration,
    /// <summary>
    /// This particle should have gas diffusion.
    /// </summary>
    Gas = 4,
}

/// <summary>
/// Flags that dictate the properties of a particle.
/// </summary>
[Flags]
public enum ParticlePropertyFlag : uint
{
    /// <summary>
    /// Absence of any properties.
    /// </summary>
    None = 0,
    /// <summary>
    /// Controls if acid melts this particle or not.
    /// </summary>
    AcidResistant = 1,
    /// <summary>
    /// Indicates this particle should be treated as a solid.
    /// </summary>
    Solid = 2,
    /// <summary>
    /// Causes this particle to entirely skip being updated in UpdateParticles().
    /// This means no custom behavior that isn't triggered by other particles will be executed.
    /// </summary>
    NoTick = 4,
    /// <summary>
    /// Indicates this particle should be treated as a gas.
    /// </summary>
    Gas = 8,
    /// <summary>
    /// Indicates this particle should be treated as a liquid.
    /// </summary>
    Liquid = 16,
}

/// <summary>
/// Flags that control how a particle renders.
/// </summary>
[Flags]
public enum ParticleRenderFlag : uint
{
    /// <summary>
    /// No draw.
    /// </summary>
    None = 0,
    /// <summary>
    /// The default layer in all it's pixel-y glory. Drawn below every other layer.
    /// </summary>
    Basic = 1,
    /// <summary>
    /// Draw to blob layer. This draws particles as 5x5 blobs.
    /// </summary>
    Blob = 2,
}