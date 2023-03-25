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

        if ((impl.MovementFlags & ParticleMovementFlag.LiquidAcceleration) != 0 && (impl.MovementFlags & ParticleMovementFlag.Gas) == 0)
        {
            var curPosI = part.Position.RoundedI();

            var pos = curPosI + (impl.RateOfGravity >= 0 ?  new Vector2i(0, 1) : new Vector2i(0, -1));

            if (!SimulationBounds.Contains(pos))
                goto failed; 
            var partAtField = GetPlayfieldEntry(pos);
            var pafImpl = Implementations[(int) partAtField.Type];
            if (partAtField.Type != ParticleType.None && (pafImpl.MovementFlags & ParticleMovementFlag.Gas) == 0)
            {
                var whichFirst = _random.Prob(0.5f);
                var liquidShiftSuccess = false;

                for (var i = 0; i < 2 && !liquidShiftSuccess && part.Type != ParticleType.None; i++)
                {
                    liquidShiftSuccess = TryMoveParticle(id, curPosI + (whichFirst ? Vector2.UnitX : -Vector2.UnitX),
                        ref part);
                    part.Velocity += new Vector2(whichFirst ? impl.DiffusionRate : -impl.DiffusionRate, 0);

                    whichFirst = !whichFirst;
                }
            }
            else
            {
                
                part.Velocity += new Vector2(0, impl.RateOfGravity);
            }
        }
        else if ((impl.MovementFlags & ParticleMovementFlag.Gas) != 0)
        {
            var xVec = _random.Next(0, 3) - 1;
            var yVec = _random.Next(0, 3) - 1;
            var vec = new Vector2i(xVec, yVec);
            
            // We don't care if this succeeds or not.
            TryMoveParticle(id, part.Position + vec, ref part);
            part.Velocity += vec * impl.DiffusionRate;
            part.Velocity += new Vector2(0, impl.RateOfGravity);
        }
        else
        {
            part.Velocity += new Vector2(0, impl.RateOfGravity);
        }
        failed:
        
        part.Velocity = part.Velocity.ClampMagnitude(impl.MaximumVelocity);
        
        if (part.Type == ParticleType.None)
            return; // Got deleted during water movement.
        
        var oldPos = part.Position;
        var newPos = part.Position + part.Velocity;
        
        if (oldPos.RoundedI() == newPos.RoundedI())
        {
            part.Position = newPos;
            return;
        }

        var didNotCollide = false;
        var rawVelocity = (newPos - oldPos).Length;
        if (rawVelocity < 1.5 || !SimulationBounds.Contains(newPos.RoundedI()))
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

            newPos = acc;
        }
        
        if (didNotCollide) return;
        
        if (part.Type == ParticleType.None || !SimulationBounds.Contains(newPos.RoundedI()))
            return;

        var collidee = GetPlayfieldEntry(newPos.RoundedI());
        var otherImpl = Implementations[(int)collidee.Type];
        
        
        if (((impl.MovementFlags & ParticleMovementFlag.Spread) != 0 || 
            (impl.MovementFlags & ParticleMovementFlag.LiquidAcceleration) != 0) && (otherImpl.MovementFlags & ParticleMovementFlag.Gas) == 0)
        {
            var whichFirst = _random.Prob(0.5f);
            var success = false;
            for (var i = 0; i < 2 && !success && part.Type != ParticleType.None; i++)
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
                    otherPart.Velocity += oldVel * (1 - impl.BounceCoefficient);
                    var newVel = oldVel * impl.BounceCoefficient;
                    if (otherPart.Position.X > part.Position.X)
                        newVel.X = -newVel.X;
                    else
                        newVel.Y = -newVel.Y;
                    part.Velocity = newVel.ClampMagnitude(impl.MaximumVelocity);
                }

            }
        }

        if (impl.MovementFlags == ParticleMovementFlag.None)
        {
            part.Velocity = Vector2.Zero;
        }
    }

    /// <summary>
    /// Attempt to move a particle to a new position.
    /// </summary>
    /// <param name="id">The ID of the particle to move.</param>
    /// <param name="newPosition">The position to move to.</param>
    /// <param name="part">A reference to the particle to move.</param>
    /// <returns>Whether or not the movement succeeded.</returns>
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
            case MovementType.PassUnder:
                // Figuring out if any particles are under us would be expensive, so we don't.
                SetPlayfieldEntry(part.Position.RoundedI(), PlayfieldEntry.None);
                // Wherever we currently are, we pass underneath it.
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
                return impl.OnMovedInto(ref Particles[partAtNew.Id], ref part, partAtNew.Id, id, newPosition.RoundedI(),
                    part.Position.RoundedI(), this);
            default:
                throw new ArgumentOutOfRangeException(nameof(movement),
                    "Someone forgot to adjust movement code for their fancy new MovementType.");
        }
    }
}