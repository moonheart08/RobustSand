using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Reflection;
using Robust.Shared.Sandboxing;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly ISandboxHelper _sandboxHelper = default!;

    /// <summary>
    /// Index of particle implementations, by ParticleType.
    /// </summary>
    public readonly ParticleImplementation[] Implementations;

    private readonly MovementType[] _movementTable;
    
    private ParticleImplementation[] InitializeImplementations()
    {
        var impls = _reflectionManager.FindTypesWithAttribute(typeof(ParticleAttribute)).ToList();
        return impls.Select(x =>
        {
            var impl = (ParticleImplementation) _sandboxHelper.CreateInstance(x);
            IoCManager.InjectDependencies(impl);
            return impl;
        }).OrderBy(x => (int)x.Type).ToArray();
    }
    
    /// <summary>
    /// Indexes into the particle movement table to dictate interaction.
    /// </summary>
    /// <param name="x">The particle being moved into.</param>
    /// <param name="y">The particle currently moving.</param>
    /// <returns>The cached MovementType.</returns>
    public MovementType GetMovementType(ParticleType x, ParticleType y)
    {
        return _movementTable[(int) y * (int) ParticleType.End + (int) x];
    }
    
    private MovementType[] InitializeMovementTable()
    {
        var movementTable = new MovementType[(int) ParticleType.End * (int) ParticleType.End];

        for (var i = 0; i < (int) ParticleType.End; i++)
        {
            for (var j = 0; j < (int) ParticleType.End; j++)
            {
                MovementType result;
                if (i == 0 || j == 0)
                    result = MovementType.Pass;
                else
                    result = Implementations[i].CanMoveThrough(Implementations[j]);

                movementTable[j * (int) ParticleType.End + i] = result;
            }
        }

        return movementTable;
    }
}