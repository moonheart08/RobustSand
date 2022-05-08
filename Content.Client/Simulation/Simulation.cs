﻿using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    public Particle[] Particles = new Particle[SimulationConfig.MaximumParticleId];
    private List<uint> _freeIds = Enumerable.Range(1,(int)SimulationConfig.MaximumParticleId-1).Reverse().Select(x => (uint)x).ToList();
    private PlayfieldEntry[] _playfield = new PlayfieldEntry[SimulationConfig.SimArea];
    
    private Box2i _simBounds = Box2i.FromDimensions(Vector2i.Zero, new Vector2i((int)SimulationConfig.SimWidth-1, (int)SimulationConfig.SimHeight-1));
    private Box2i _innerSimBounds = Box2i.FromDimensions(new Vector2i(4, 4), new Vector2i((int)SimulationConfig.SimWidth - 5, (int)SimulationConfig.SimHeight - 5));

    private uint _lastActiveParticle = 0;

    public Simulation()
    {
        IoCManager.InjectDependencies(this);
        Implementations = InitializeImplementations();
        _movementTable = InitializeMovementTable();
        Particles.Initialize();

        for (var y = SimulationConfig.SimHeight - 16; y < (SimulationConfig.SimHeight - 5); y++)
        {
            for (var x = 5; x < SimulationConfig.SimWidth-5; x++)
            {
                TrySpawnParticle(new Vector2i(x, (int)y), ParticleType.WALL, out _);
            }
        }

        TrySpawnParticle(new Vector2i(255, 255), ParticleType.SPAWNER, out var spawnerId); // The world shall have sand.
        Particles[spawnerId!.Value].Variable1 = (int) ParticleType.SAND;
        
        TrySpawnParticle(new Vector2i(205, 255), ParticleType.SPAWNER, out var spawnerId2); // The world shall have sand.
        Particles[spawnerId2!.Value].Variable1 = (int) ParticleType.SAND;
        
        TrySpawnParticle(new Vector2i(230, 255), ParticleType.SPAWNER, out var spawnerId3); // The world shall have sand.
        Particles[spawnerId3!.Value].Variable1 = (int) ParticleType.WATER;
    }

    public void RunFrame()
    {
        UpdateParticles();
        CleanupNewFrame();
    }

    private void CleanupNewFrame()
    {
        for (var i = 0; i < SimulationConfig.SimArea; i++)
        {
            _playfield[i] = PlayfieldEntry.None;
        }

        uint newLastActive = 0;
        for (uint i = 0; i <= _lastActiveParticle; i++)
        {
            if (Particles[i].Type == ParticleType.NONE) continue;
            
            ref var part = ref Particles[i];
            var position = part.Position.RoundedI();
            SetPlayfieldEntry(position, new PlayfieldEntry(part.Type, i));
            if (i > newLastActive)
                newLastActive = i;
        }

        _lastActiveParticle = newLastActive;
    }

    private void UpdateParticles()
    {
        // This comment is left here in memory of 12 lines of variable declarations.

        for (uint i = 0; i <= _lastActiveParticle; i++)
        {
            if (Particles[i].Type == ParticleType.NONE)
                continue;

            ref var part = ref Particles[i];
            var partPos = part.Position.RoundedI();

            if (!_innerSimBounds.Contains(partPos))
            {
                DeleteParticle(i, partPos, ref part);
                continue; // Particle's deleted, moving on.
            }
            
            Implementations[(int)part.Type].Update(ref part, i, partPos, this);

            ProcessParticleMovement(i, ref part);
        }
    }

    public PlayfieldEntry GetPlayfieldEntry(Vector2i position)
    {
        return _playfield[position.Y * SimulationConfig.SimWidth + position.X];
    }

    private void SetPlayfieldEntry(Vector2i position, PlayfieldEntry id)
    {
        _playfield[position.Y * SimulationConfig.SimWidth + position.X] = id;
    }

    public bool TrySpawnParticle(Vector2i position, ParticleType type, out uint? id)
    {
        if (_freeIds.Count == 0)
        {
            id = null;
            return false;
        }

        var newId = _freeIds[^1];
        DebugTools.Assert(Particles[newId].Type == ParticleType.NONE);
        var part = new Particle(position, type);
        if (Implementations[(int) type].Spawn(ref part))
        {
            id = newId;
            _freeIds.Pop();
            Particles[newId] = part;
            SetPlayfieldEntry(position, new PlayfieldEntry(type, id.Value));

            if (_lastActiveParticle < id)
                _lastActiveParticle = id.Value;
            return true;
        }

        id = null;
        return false;
    }

    public void DeleteParticle(uint id, Vector2i position, ref Particle particle)
    {
        DebugTools.Assert(Particles[id].Type != ParticleType.NONE);
        ref var part = ref Particles[id];
        Implementations[(int)part.Type].Delete(ref part);
        _freeIds.Add(id);
        part.Type = ParticleType.NONE;

        if (_simBounds.Contains(position))
            SetPlayfieldEntry(position, PlayfieldEntry.None);
    }
}