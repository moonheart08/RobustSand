using System;
using Content.Client.Simulation.Saving;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{

    public Simulation(SaveData saveData)
    {
        IoCManager.InjectDependencies(this);
        Implementations = InitializeImplementations();
        _movementTable = InitializeMovementTable();
        
        var parts = saveData.ReadParticleData();

        foreach (var part in parts)
        {
            if (!SaveBuildParticle(part, out _))
                throw new ArgumentException(
                    "Failed to load the save due to ID exhaustion. Are you sure this is compatible?");
        }
        
        CleanupNewFrame();
    }

    private bool SaveBuildParticle(Particle part, out uint? id)
    {
        if (!_freeIds.TryPop(out var newId))
        {
            id = null;
            return false;
        }

        Particles[newId] = part;
        if (_lastActiveParticle < newId)
            _lastActiveParticle = newId;

        id = newId;
        // The cleanup code will run later and actually populate the playfield for us.
        return true;
    }
}