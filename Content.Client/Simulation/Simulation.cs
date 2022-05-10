using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public Particle[] Particles = new Particle[SimulationConfig.MaximumParticleId];
    private ConcurrentStack<uint> _freeIds = new (Enumerable.Range(1,(int)SimulationConfig.MaximumParticleId-1).Reverse().Select(x => (uint)x).ToList());
    private PlayfieldEntry[] _playfield = new PlayfieldEntry[SimulationConfig.SimArea];
    
    public Box2i SimulationBounds = Box2i.FromDimensions(Vector2i.Zero, new Vector2i((int)SimulationConfig.SimWidth-1, (int)SimulationConfig.SimHeight-1));
    public Box2i InnerSimulationBounds = Box2i.FromDimensions(new Vector2i(4, 4), new Vector2i((int)SimulationConfig.SimWidth - 5, (int)SimulationConfig.SimHeight - 5));

    public uint LiveParticles { get; private set; }
    
    private uint _lastActiveParticle = 0;

    public Simulation()
    {
        IoCManager.InjectDependencies(this);
        Implementations = InitializeImplementations();
        _movementTable = InitializeMovementTable();

        DebugTools.Assert(SimulationBounds.Contains(Vector2i.Zero));
        DebugTools.Assert(SimulationBounds.Contains(new Vector2i((int)SimulationConfig.SimWidth-1, (int)SimulationConfig.SimHeight-1)));
        DebugTools.Assert(!SimulationBounds.Contains(new Vector2i((int)SimulationConfig.SimWidth, (int)SimulationConfig.SimHeight)));
        DebugTools.Assert(!SimulationBounds.Contains(-Vector2i.One));

        for (var y = SimulationConfig.SimHeight - 16; y < (SimulationConfig.SimHeight - 5); y++)
        {
            for (var x = 5; x < SimulationConfig.SimWidth-5; x++)
            {
                TrySpawnParticle(new Vector2i(x, (int)y), ParticleType.Wall, out _);
            }
        }

        TrySpawnParticle(new Vector2i(255, 255), ParticleType.Spawner, out var spawnerId); // The world shall have sand.
        Particles[spawnerId!.Value].Variable1 = (int) ParticleType.Sand;
        
        TrySpawnParticle(new Vector2i(205, 255), ParticleType.Spawner, out var spawnerId2); // The world shall have sand.
        Particles[spawnerId2!.Value].Variable1 = (int) ParticleType.Sand;
        
        TrySpawnParticle(new Vector2i(230, 255), ParticleType.Spawner, out var spawnerId3); // The world shall have sand.
        Particles[spawnerId3!.Value].Variable1 = (int) ParticleType.Water;
    }
    
    private Task[] _tasks = new Task[(SimulationConfig.SimWidthChunks / 2) * (SimulationConfig.SimHeightChunks / 2)];

    public void RunFrame()
    {
        CleanupNewFrame();
        //UpdateParticles(SimulationBounds);

        if (LiveParticles < SimulationConfig.ConcurrencyThresh)
        {
            // just update the whole playfield and be done with it.
            UpdateParticles(SimulationBounds);
            return;
        }
        
        

        var idx = 0;
        for (int step = 0; step < 4; step++)
        {
            for (int x = 0; x < (SimulationConfig.SimWidthChunks / 2); x++)
            {
                for (int y = 0; y < (SimulationConfig.SimHeightChunks / 2); y++)
                {
                    var chunk = GetChunk(step, x, y);
                    _tasks[idx] = Task.Run(() => UpdateParticles(chunk));
                    idx++;
                }
            }
            Task.WaitAll(_tasks);
            idx = 0;
        }

        
    }

    public Box2i GetChunk(int step, int x, int y)
    {
        bool offsetX = step % 2 != 0;
        bool offsetY = step >= 2;

        var chunkSize = (int)SimulationConfig.ChunkSize;

        Vector2i pos = new Vector2i((x * 2 * chunkSize) + (offsetX ? chunkSize : 0), (y * 2 * chunkSize) + (offsetY ? chunkSize : 0));

        return Box2i.FromDimensions(pos, Vector2i.One * chunkSize);
    }

    private void CleanupNewFrame()
    {
        for (var i = 0; i < SimulationConfig.SimArea; i++)
        {
            _playfield[i] = PlayfieldEntry.None;
        }

        uint newLastActive = 0;
        uint liveCount = 0;
        for (uint i = 0; i <= _lastActiveParticle; i++)
        {
            if (Particles[i].Type == ParticleType.None) 
                continue;
            
            ref var part = ref Particles[i];
            part.AlreadyUpdated = false;
            var position = part.Position.RoundedI();
            var entry = new PlayfieldEntry(part.Type, i);
            SetPlayfieldEntry(position, entry);
            DebugTools.Assert(GetPlayfieldEntry(position) == entry);
            if (i > newLastActive)
                newLastActive = i;

            liveCount += 1;
        }

        LiveParticles = liveCount;

        _lastActiveParticle = newLastActive;
    }

    private void UpdateParticles(Box2i region)
    {
        // This comment is left here in memory of 12 lines of variable declarations.

        for (uint i = 0; i <= _lastActiveParticle; i++)
        {
            ref var part = ref Particles[i];
            var impl = Implementations[(int) part.Type];
            if ((impl.PropertyFlags & ParticlePropertyFlag.NoTick) != 0)
                continue; // nothing to see here.
            
            if (part.Type == ParticleType.None || !region.Contains(part.Position.RoundedI()) || part.AlreadyUpdated)
                continue;
            
            part.AlreadyUpdated = true;
            var partPos = part.Position.RoundedI();

            if (!InnerSimulationBounds.Contains(partPos))
            {
                DeleteParticle(i, partPos, ref part);
                continue; // Particle's deleted, moving on.
            }
            
            impl.Update(ref part, i, partPos, this);
            
            if (part.Type == ParticleType.None)
                continue;

            ProcessParticleMovement(i, ref part);
        }
        

    }
}