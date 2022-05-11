using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;

namespace Content.Client.Simulation.Saving;

public sealed class SaveData
{
    public const int CurrentVersion = 2;
    
    public int Version;
    public Memory<byte> ParticleData;

    private Dictionary<ushort, ParticleType>? _particleTypeTable;

    // TODO: The save format should probably include an index of particle types.
    public SaveData(Simulation sim)
    {
        var parts = sim.Particles.ToList(); // Make a copy, the sim may still be running.
        ParticleData = new byte[Particle.ParticleSaveSize * parts.Count(x => x.Type != ParticleType.None)];

        var memSpan = ParticleData.Span;
        var idx = 0;
        foreach (var particle in parts)
        {
            if (particle.Type == ParticleType.None)
                continue;
            
            particle.ByteSerialize(memSpan.Slice(idx, Particle.ParticleSaveSize));
            idx += Particle.ParticleSaveSize;
        }
        
        Logger.InfoS("save",$"Serialized {idx} bytes of particle data, out of {ParticleData.Length} bytes allocated.");

        Version = CurrentVersion;
    }

    public byte[] WriteFile()
    {
        var nameTypeDict = Enum.GetValues<ParticleType>().ToDictionary(x => Enum.GetName(x)!, x => (ushort)x);
        var dictLen = SerializationHelpers.ExpectedTypeDictLength(nameTypeDict);
        Memory<byte> outp = new byte[ParticleData.Length + dictLen + 8];
        var outSpan = outp.Span;
        var pos = 0;
        BitConverter.TryWriteBytes(outSpan.Slice(pos, sizeof(int)), Version);
        pos += 4;
        BitConverter.TryWriteBytes(outSpan.Slice(pos, sizeof(int)), ParticleData.Length);
        pos += 4;
        SerializationHelpers.SerializeTypeDict(outSpan.Slice(pos, dictLen), nameTypeDict);
        pos += dictLen;
        ParticleData.Span.TryCopyTo(outSpan.Slice(pos));

        var arr = outp.ToArray();
        
        Logger.InfoS("save", $"Wrote a {outp.Length} byte save file with hash {SerializationHelpers.CalcCRC32(arr):x8}.");
        
        return arr;
    }

    public SaveData(byte[] data)
    {
        Memory<byte> dataMem = data;
        var dataSpan = dataMem.Span;
        var version = BitConverter.ToInt32(dataSpan.Slice(0, 4));
        if (version is > CurrentVersion or > 100)
            throw new ArgumentException($"Unrecognized save version {version}");
        Version = version;

        var hash = SerializationHelpers.CalcCRC32(data);
        Logger.InfoS("save", $"Reading a v{version} save file with hash {hash:x8}...");
        
        ReadBackVersion0xx(dataMem, version, hash);
    }

    private void ReadBackVersion0xx(Memory<byte> data, int version, uint hash)
    {
        var pos = 4;
        var particleDataLen = BitConverter.ToInt32(data.Slice(pos, 4).Span);
        pos += 4;
        if (Version >= 2) // Version 2 introduced the particle type lookup table and CRC hashes.
        {
            var (dict, dictLen) = SerializationHelpers.DeserializeTypeDict(data.Slice(pos).Span);
            var nameTypeDict = Enum.GetValues<ParticleType>().ToDictionary(x => Enum.GetName(x)!, x => x);

            var finalDict = new Dictionary<ushort, ParticleType>();
            foreach (var (key, value) in nameTypeDict)
            {
                if (!dict.ContainsKey(key))
                    continue;
                
                finalDict.Add(dict[key], value);
            }

            _particleTypeTable = finalDict;
            pos += dictLen;
        }
        
        ParticleData = data.Slice(pos, particleDataLen); 
        Logger.InfoS("save", $"Read {particleDataLen} bytes of particle data.");
    }

    public Particle[] ReadParticleData()
    {
        var particles = new List<Particle>(ParticleData.Length / Particle.ParticleSaveSize);

        var pos = 0;

        var pdSpan = ParticleData.Span;

        while (pos < ParticleData.Length)
        {
            particles.Add(new Particle(pdSpan.Slice(pos, Particle.ParticleSaveSize), Version, _particleTypeTable));
            pos += Particle.ParticleSaveSize;
        }

        Logger.InfoS("save", $"Read {particles.Count} particles from file.");
        return particles.ToArray();
    }
}