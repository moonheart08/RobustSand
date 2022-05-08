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

        bool didNotCollide = false;
        var rawVelocity = (newPos - oldPos).Length;
        if (rawVelocity < 1.5)
        {
            didNotCollide = TryMoveParticle(id, newPos, ref part);
        }
        else
        {
            var stepsToDo = Math.Min((int) rawVelocity, (int)SimulationConfig.MaximumCollisionSteps);
            var delta = (newPos - oldPos) / stepsToDo;
            var acc = oldPos;
            for (var step = 0; step < stepsToDo; step++)
            {
                acc += delta;
                didNotCollide = TryMoveParticle(id, acc, ref part);
                if (!didNotCollide)
                    break;
            }
        }
        

        var collidee = GetPlayfieldEntry(newPos.RoundedI());
        var otherImpl = Implementations[(int)collidee.Type];

        if (!didNotCollide)
        {
            if ((impl.MovementFlags & ParticleMovementFlag.Spread) != 0 || 
                (impl.MovementFlags & ParticleMovementFlag.Liquid) != 0)
            {
                var whichFirst = _random.Prob(0.5f);
                var success = false;
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

                if (!didNotCollide)
                {
                    
                    if ((otherImpl.PropertyFlags & ParticlePropertyFlag.Solid) != 0)
                        part.Velocity = Vector2.Zero;
                    else
                    {
                        var oldVel = part.Velocity;
                        ref var otherPart = ref Particles[collidee.Id];
                        // TODO: Account for weight in this.
                        otherPart.Velocity += oldVel / 2;
                        part.Velocity = oldVel / 2;
                    }

                }
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
        var movement = GetMovementType(partAtNew.Type, part.Type);

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
            case MovementType.Custom:
                var impl = Implementations[(int) partAtNew.Type];
                return impl.DoMovement(ref Particles[partAtNew.Id], ref part, partAtNew.Id, id, newPosition.RoundedI(),
                    part.Position.RoundedI(), this);
            default:
                throw new ArgumentOutOfRangeException(nameof(movement),
                    "Someone forgot to adjust movement code for their fancy new MovementType.");
        }
    }
}