using System;

namespace Content.Client.Simulation;

public static class SimulationConfig
{
    /// <summary>
    /// The width of the simulation playfield.
    /// </summary>
    public const uint SimWidth = 512;
    /// <summary>
    /// The height of the simulation playfield.
    /// </summary>
    public const uint SimHeight = 512;

    /// <summary>
    /// The calculated area of the simulation playfield.
    /// </summary>
    public const uint SimArea = SimWidth * SimHeight;
    
    /// <summary>
    /// The maximum particle type ID permitted.
    /// </summary>
    /// <remarks>
    /// This must always be a power of two minus 1 (aka a bitmask)
    /// </remarks>
    public const uint MaximumParticleType = 1023;

    public const uint ParticleTypeBits = 10; // TODO: make this use calculated from MaximumParticleType
    
    /// <summary>
    /// The maximum particle ID permitted.
    /// </summary>
    /// <remarks>
    /// This is derived from the maximum type ID, allowing more types reduces the number of particles that can be in play at once.
    /// </remarks>
    public const uint MaximumParticleId = uint.MaxValue / (MaximumParticleType + 1) - 1;

    /// <summary>
    /// Maximumm number of steps collision may take checking for blockers.
    /// Determines how easily particles can ignore the rules of physics and tunnel through walls.
    /// </summary>
    public const uint MaximumCollisionSteps = 4;
}