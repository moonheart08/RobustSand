using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public readonly record struct PlayfieldEntry
{
    public static PlayfieldEntry None => new PlayfieldEntry(ParticleType.NONE, 0);
    
    private readonly uint _inner;

    public ParticleType Type => (ParticleType)(_inner & SimulationConfig.MaximumParticleType);
    public uint Id => (_inner & (~SimulationConfig.MaximumParticleType)) >> (int)SimulationConfig.ParticleTypeBits;

    public PlayfieldEntry(ParticleType type, uint id)
    {
        _inner = (uint) type | (id << (int) SimulationConfig.ParticleTypeBits);
    }
};