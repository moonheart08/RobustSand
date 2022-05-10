using System.Runtime.CompilerServices;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PlayfieldEntry GetPlayfieldEntry(Vector2i position)
    {
        return _playfield[position.Y * SimulationConfig.SimWidth + position.X];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetPlayfieldEntry(Vector2i position, PlayfieldEntry id)
    {
        _playfield[position.Y * SimulationConfig.SimWidth + position.X] = id;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public bool TrySpawnParticle(Vector2i position, ParticleType type, out uint? id)
    {
        // TODO: Is this even thread safe???? (Probably not!) (definitely not)
        if (!_freeIds.TryPop(out var newId))
        {
            id = null;
            return false;
        }
        
        DebugTools.Assert(Particles[newId].Type == ParticleType.None);
        var part = new Particle(position, type);
        if (Implementations[(int) type].Spawn(ref part))
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

    public void DeleteParticle(uint id, Vector2i position, ref Particle particle)
    {
        DebugTools.Assert(Particles[id].Type != ParticleType.None);
        Implementations[(int) particle.Type].Delete(ref particle);
        _freeIds.Push(id);
        particle.Type = ParticleType.None;

        if (SimulationBounds.Contains(position))
            SetPlayfieldEntry(position, PlayfieldEntry.None);
    }

    public void ChangeParticleType(uint id, Vector2i position, ref Particle particle, ParticleType newType)
    {
        particle.Type = newType;
        Implementations[(int) newType].ChangedType(ref particle, id, position, this, particle.Type);
        SetPlayfieldEntry(position, new PlayfieldEntry(newType, id)); // Make sure particles around us get the memo.
    }
}