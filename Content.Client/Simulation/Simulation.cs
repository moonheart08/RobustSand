using System;
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
    private List<uint> _freeIds = Enumerable.Range(1,(int)SimulationConfig.MaximumParticleId-1).Reverse().Select(x => (uint)x).ToList();
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
        CleanupNewFrame();
        var taskList = new List<Task>();
        //UpdateParticles(SimulationBounds);

        Task[] tasks = new Task[4 * (SimulationConfig.SimWidth / 128) * (SimulationConfig.SimHeight / 128)];

        var idx = 0;
        for (int step = 0; step < 4; step++)
        {
            for (int x = 0; x < SimulationConfig.SimWidth / 128; x++)
            {
                for (int y = 0; y < SimulationConfig.SimHeight / 128; y++)
                {
                    var chunk = GetChunk(step, x, y);
                    tasks[idx] = Task.Run(() => UpdateParticles(chunk));
                    idx++;
                }
            }
        }

        Task.WaitAll(tasks);
    }

    public Box2i GetChunk(int step, int x, int y)
    {
        const uint totalX = SimulationConfig.SimWidth / 64;
        const uint totalY = SimulationConfig.SimHeight / 64;
        bool offsetX = step % 2 != 0;
        bool offsetY = step >= 2;

        Vector2i pos = new Vector2i((x * 2 * 64) + (offsetX ? 64 : 0), (y * 2 * 64) + (offsetY ? 64 : 0));

        return Box2i.FromDimensions(pos, Vector2i.One * 63);
    }

    private void CleanupNewFrame()
    {
        /*for (var i = 0; i < SimulationConfig.SimArea; i++)
        {
            _playfield[i] = PlayfieldEntry.None;
        }*/

        uint newLastActive = 0;
        uint liveCount = 0;
        for (uint i = 0; i <= _lastActiveParticle; i++)
        {
            if (Particles[i].Type == ParticleType.NONE) 
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
            if (Particles[i].Type == ParticleType.NONE || !region.Contains(Particles[i].Position.RoundedI()) || Particles[i].AlreadyUpdated)
                continue;

            ref var part = ref Particles[i];
            part.AlreadyUpdated = true;
            var partPos = part.Position.RoundedI();

            if (!InnerSimulationBounds.Contains(partPos))
            {
                DeleteParticle(i, partPos, ref part);
                continue; // Particle's deleted, moving on.
            }
            
            Implementations[(int)part.Type].Update(ref part, i, partPos, this);
            
            if (part.Type == ParticleType.NONE)
                continue;

            ProcessParticleMovement(i, ref part);
        }
        

    }
}