using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    /// <summary>
    /// Get the playfield entry at the given position.
    /// </summary>
    /// <param name="position">Position to look up.</param>
    /// <returns>Playfield entry.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PlayfieldEntry GetPlayfieldEntry(Vector2i position)
    {
        return _playfield[position.Y * SimulationConfig.SimWidth + position.X];
    }
    
    /// <summary>
    /// Set the playfield entry at the given position.
    /// </summary>
    /// <param name="position">Position to place at.</param>
    /// <param name="entry">Entry to place.</param>
    /// <remarks>
    /// It is strongly recommended to not use this to create phantom particles, no code is designed for that.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetPlayfieldEntry(Vector2i position, PlayfieldEntry entry)
    {
        _playfield[position.Y * SimulationConfig.SimWidth + position.X] = entry;
    }

    /// <summary>
    /// Attempts to spawn a particle at the given location.
    /// </summary>
    /// <param name="position">The location to spawn a particle at.</param>
    /// <param name="type">The type of particle to spawn.</param>
    /// <param name="id">The ID of the spawned particle, if any.</param>
    /// <returns>Whether or not spawning succeeded.</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool TrySpawnParticle(Vector2i position, ParticleType type, [NotNullWhen(true)] out uint? id)
    {
        // TODO: Is this even thread safe???? (Probably not!) (definitely not)
        if (!_freeIds.TryPop(out var newId))
        {
            id = null;
            return false;
        }
        
        DebugTools.Assert(Particles[newId].Type == ParticleType.None);
        var part = new Particle(position, type);
        if (Implementations[(int) type].OnSpawn(ref part))
        {
            id = newId;
            Particles[newId] = part;
            SetPlayfieldEntry(part.Position.RoundedI(), new PlayfieldEntry(type, id.Value));
            DebugTools.Assert(GetPlayfieldEntry(part.Position.RoundedI()) == new PlayfieldEntry(type, id.Value));

            if (_lastActiveParticle < id)
                _lastActiveParticle = id.Value;
            return true;
        }

        _freeIds.Push(newId);
        id = null;
        return false;
    }

    /// <summary>
    /// Deletes the given particle from the sim, making a best-effort attempt to clean up using the provided position.
    /// </summary>
    /// <param name="id">The ID of the particle to delete.</param>
    /// <param name="position">The expected position of the particle.</param>
    /// <param name="particle">A reference to the particle to delete.</param>
    /// <remarks>If the position is incorrect, the playfield will not be cleaned up correctly which can and will cause fuckery.</remarks>
    public void DeleteParticle(uint id, Vector2i position, ref Particle particle)
    {
        DebugTools.Assert(Particles[id].Type != ParticleType.None);
        Implementations[(int) particle.Type].OnDelete(ref particle);
        _freeIds.Push(id);
        particle.Type = ParticleType.None;

        if (SimulationBounds.Contains(position))
            SetPlayfieldEntry(position, PlayfieldEntry.None);
    }

    /// <summary>
    /// Change a particle's type.
    /// </summary>
    /// <param name="id">The ID of the particle to change the type of.</param>
    /// <param name="position">The expected position of the particle.</param>
    /// <param name="particle">A reference to the particle to change type for.</param>
    /// <param name="newType">The new type to change to.</param>
    /// <remarks>If the position is incorrect, the playfield will not be cleaned up correctly which can and will cause fuckery.</remarks>
    public void ChangeParticleType(uint id, Vector2i position, ref Particle particle, ParticleType newType)
    {
        particle.Type = newType;
        var impl = Implementations[(int) newType];
        Implementations[(int) newType].OnChangedIntoType(ref particle, id, position, this, particle.Type);
        
        if ((impl.PropertyFlags & ParticlePropertyFlag.Solid) != 0)
            particle.Velocity = Vector2.Zero; // nuh-uh-uh, don't you dare.
        
        SetPlayfieldEntry(position, new PlayfieldEntry(newType, id)); // Make sure particles around us get the memo.
    }
}