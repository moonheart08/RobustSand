using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public readonly record struct PlayfieldEntry
{
    public static PlayfieldEntry None => new PlayfieldEntry(ParticleType.None, 0);
    
    private readonly uint _inner;

    /// <summary>
    /// The type of the particle in this entry. A value of None indicates the absence of any particle.
    /// </summary>
    public ParticleType Type => (ParticleType)(_inner & SimulationConfig.MaximumParticleType);
    
    /// <summary>
    /// The ID of the particle in this entry. A value of 0 indicates the absence of any particle.
    /// </summary>
    public uint Id => (_inner & (~SimulationConfig.MaximumParticleType)) >> (int)SimulationConfig.ParticleTypeBits;

    /// <summary>
    /// Construct an entry from a type and ID.
    /// </summary>
    /// <param name="type">Type of the particle for this entry.</param>
    /// <param name="id">ID of the particle for this entry.</param>
    public PlayfieldEntry(ParticleType type, uint id)
    {
        _inner = (uint) type | (id << (int) SimulationConfig.ParticleTypeBits);
    }
};