using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Simulation;

/// <summary>
/// The particle data structure.
/// </summary>
public struct Particle
{
    /// <summary>
    /// Particle position.
    /// </summary>
    public Vector2 Position;
    /// <summary>
    /// Particle velocity.
    /// </summary>
    public Vector2 Velocity;
    public int Variable1;
    public int Variable2;
    public int Variable3;
    public int Variable4;
    /// <summary>
    /// Temperature in kelvin.
    /// </summary>
    public float Temperature;
    /// <summary>
    /// Type of the particle.
    /// </summary>
    public ParticleType Type = ParticleType.None;
    public bool AlreadyUpdated;

    /// <summary>
    /// Constructs a particle
    /// </summary>
    /// <param name="position">Position to construct it at.</param>
    public Particle(Vector2 position, ParticleType type) : this()
    {
        Position = position;
        Type = type;
        Velocity = Vector2.Zero;
        Variable1 = 0;
        Variable2 = 0;
        Variable3 = 0;
        Variable4 = 0;
        Temperature = SimulationConfig.RoomTemperature;
        AlreadyUpdated = false;
    }

    public const int ParticleSaveSize = 34;
    
    /// <summary>
    /// Serializes to the given byte buffer. Expects 64 bytes.
    /// </summary>
    /// <param name="buffer">Buffer to write to</param>
    /// <returns>Bytes written</returns>
    public int ByteSerialize(Span<byte> buffer)
    {
        buffer.Clear();
        var pos = 0;
        // Serialize type...
        BitConverter.TryWriteBytes(buffer.Slice(pos, 2), (ushort)Type);
        pos += 2;
        // Serialize position and velocity...
        BitConverter.TryWriteBytes(buffer.Slice(pos+0, 4), Position.X);
        BitConverter.TryWriteBytes(buffer.Slice(pos+4, 4), Position.Y);
        BitConverter.TryWriteBytes(buffer.Slice(pos+8, 4), Velocity.X);
        BitConverter.TryWriteBytes(buffer.Slice(pos+12, 4), Velocity.Y);
        pos += 16;
        // Serialize variables...
        BitConverter.TryWriteBytes(buffer.Slice(pos+0, 4), Variable1);
        BitConverter.TryWriteBytes(buffer.Slice(pos+4, 4), Variable2);
        BitConverter.TryWriteBytes(buffer.Slice(pos+8, 4), Variable3);
        BitConverter.TryWriteBytes(buffer.Slice(pos+12, 4), Variable4);
        pos += 16;
        // Serialize our temperature.
        BitConverter.TryWriteBytes(buffer.Slice(pos + 0, 4), Temperature);
        pos += 4;
        // Done.
        return pos;
    }

    /// <summary>
    /// Constructs a particle from saved data.
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="saveVersion"></param>
    public Particle(Span<byte> buffer, int saveVersion, Dictionary<ushort, ParticleType>? particleTypeTable)
    {
        var pos = 0;
        var rawType = BitConverter.ToUInt16(buffer.Slice(pos, 2));
        if (particleTypeTable == null)
            Type = (ParticleType) rawType;
        else if (particleTypeTable.ContainsKey(rawType))
        {
            Type = particleTypeTable[rawType];
        }
        else
        {
            Type = ParticleType.Sand; // Welp, we somehow got a nonexistent element. Convert it to sand so we can still load the save.
        }
        DebugTools.Assert(Type < ParticleType.End, $"Expected a valid type and got {(ushort)Type}");
        pos += 2;
        Position.X = BitConverter.ToSingle(buffer.Slice(pos + 0, 4));
        Position.Y = BitConverter.ToSingle(buffer.Slice(pos + 4, 4));
        Velocity.X = BitConverter.ToSingle(buffer.Slice(pos + 8, 4));
        Velocity.Y = BitConverter.ToSingle(buffer.Slice(pos + 12, 4));
        pos += 16;
        Variable1 = BitConverter.ToInt32(buffer.Slice(pos + 0, 4));
        Variable2 = BitConverter.ToInt32(buffer.Slice(pos + 4, 4));
        Variable3 = BitConverter.ToInt32(buffer.Slice(pos + 8, 4));
        Variable4 = BitConverter.ToInt32(buffer.Slice(pos + 12, 4));
        pos += 16;
        if (buffer.Length > pos)
        {
            Temperature = BitConverter.ToInt32(buffer.Slice(pos + 0, 4));
        }
        else
        {
            Temperature = SimulationConfig.RoomTemperature;
        }

        AlreadyUpdated = false;
    }
}