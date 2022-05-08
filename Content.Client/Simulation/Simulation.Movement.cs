using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    private void ProcessParticleMovement(uint id, ref Particle part)
    {
        var impl = Implementations[(int) part.Type];
        part.Velocity += new Vector2(0, impl.RateOfGravity);
        if ((impl.MovementFlags & ParticleMovementFlag.Liquid) != 0)
        {
            var curPosI = part.Position.RoundedI();

            var pos = curPosI + new Vector2i(0, 1);
            if (!SimulationBounds.Contains(pos))
                return;

            var entry = GetPlayfieldEntry(pos);
            if (entry.Type != ParticleType.NONE)
            {
                var whichFirst = _random.Prob(0.5f);
                var liquidShiftSuccess = false;

                for (var i = 0; i < 2 && !liquidShiftSuccess && part.Type != ParticleType.NONE; i++)
                {
                    liquidShiftSuccess = TryMoveParticle(id, curPosI + (whichFirst ? Vector2.UnitX : -Vector2.UnitX),
                        ref part);

                    whichFirst = !whichFirst;
                }
            }
        }
        
        var oldPos = part.Position;
        var newPos = part.Position + part.Velocity;
        if (oldPos.RoundedI() == newPos.RoundedI())
        {
            part.Position = newPos;
            return;
        }

        var success = TryMoveParticle(id, newPos, ref part);

        if (!success)
        {
            if ((impl.MovementFlags & ParticleMovementFlag.Spread) != 0 || 
                (impl.MovementFlags & ParticleMovementFlag.Liquid) != 0)
            {
                var whichFirst = _random.Prob(0.5f);
                for (var i = 0; i < 2 && !success && part.Type != ParticleType.NONE; i++)
                {
                    if (whichFirst)
                    {
                        success = TryMoveParticle(id, newPos + Vector2.UnitX, ref part);
                    }
                    else
                    {
                        success = TryMoveParticle(id, newPos - Vector2.UnitX, ref part);
                    }
                    whichFirst = !whichFirst;
                }
                
                if (!success)
                    part.Velocity = Vector2.Zero;
            }

            if (impl.MovementFlags == ParticleMovementFlag.None)
            {
                part.Velocity = Vector2.Zero;
            }
        }
    }

    public bool TryMoveParticle(uint id, Vector2 newPosition, ref Particle part)
    {
        if (!SimulationBounds.Contains(newPosition.RoundedI()))
        {
            DeleteParticle(id, part.Position.RoundedI(), ref part);
            return true;
        }

        var partAtNew = GetPlayfieldEntry(newPosition.RoundedI());
        var movement = GetMovementType(part.Type, partAtNew.Type);

        switch (movement)
        {
            case MovementType.Block:
                return false; // We simply cannot move here.
            case MovementType.Pass:
                // Figuring out if any particles are under us would be expensive, so we don't.
                SetPlayfieldEntry(part.Position.RoundedI(), PlayfieldEntry.None);
                SetPlayfieldEntry(newPosition.RoundedI(), new PlayfieldEntry(part.Type, id));
                part.Position = newPosition;
                return true;
            case MovementType.Swap:
                SetPlayfieldEntry(newPosition.RoundedI(), new PlayfieldEntry(part.Type, id));
                SetPlayfieldEntry(part.Position.RoundedI(), partAtNew);
                ref var swappedPart = ref Particles[partAtNew.Id];
                swappedPart.Position = part.Position;
                part.Position = newPosition;
                return true;
            default:
                throw new ArgumentOutOfRangeException(nameof(movement),
                    "Someone forgot to adjust movement code for their fancy new MovementType.");
        }
    }
}