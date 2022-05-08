using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Reflection;
using Robust.Shared.Sandboxing;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly ISandboxHelper _sandboxHelper = default!;

    public readonly ParticleImplementation[] Implementations;

    private readonly MovementType[] _movementTable;
    
    private ParticleImplementation[] InitializeImplementations()
    {
        var impls = _reflectionManager.FindTypesWithAttribute(typeof(ParticleAttribute)).ToList();
        return impls.Select(x => (ParticleImplementation) _sandboxHelper.CreateInstance(x)).OrderBy(x => (int)x.Type).ToArray();
    }

    public MovementType GetMovementType(ParticleType x, ParticleType y)
    {
        return _movementTable[(int) y * (int) ParticleType.END + (int) x];
    }
    
    private MovementType[] InitializeMovementTable()
    {
        var movementTable = new MovementType[(int) ParticleType.END * (int) ParticleType.END];

        for (var i = 0; i < (int) ParticleType.END; i++)
        {
            for (var j = 0; j < (int) ParticleType.END; j++)
            {
                MovementType result;
                if (i == 0 || j == 0)
                    result = MovementType.Pass;
                else
                    result = Implementations[i].CanMoveThrough(Implementations[j]);

                movementTable[j * (int) ParticleType.END + i] = result;
            }
        }

        return movementTable;
    }
}