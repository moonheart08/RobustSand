using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Simulation.ParticleKinds.Abstract;

namespace Content.Client.Simulation.Saving;

public sealed class SaveData
{
    public const int CurrentVersion = 1;
    
    public int Version;
    public Memory<byte> ParticleData;

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

            Logger.Debug($"{idx}, {memSpan.Length}");
            particle.ByteSerialize(memSpan.Slice(idx, Particle.ParticleSaveSize));
            idx += Particle.ParticleSaveSize;
        }

        Version = CurrentVersion;
    }

    public byte[] WriteFile()
    {
        // int version, int length.
        Memory<byte> outp = new byte[ParticleData.Length + 8];
        var outSpan = outp.Span;

        BitConverter.TryWriteBytes(outSpan.Slice(0, 4), Version);
        BitConverter.TryWriteBytes(outSpan.Slice(4, 4), ParticleData.Length);
        ParticleData.Span.TryCopyTo(outSpan.Slice(8));
        
        return outp.ToArray();
    }

    public SaveData(byte[] data)
    {
        Memory<byte> dataMem = data;
        var dataSpan = dataMem.Span;
        var version = BitConverter.ToInt32(dataSpan.Slice(0, 4));
        if (version is > CurrentVersion or > 100)
            throw new ArgumentException($"Unrecognized save version {version}");
        
        ReadBackVersion1xx(dataMem);
    }

    private void ReadBackVersion1xx(Memory<byte> data)
    {
        var particleDataLen = BitConverter.ToInt32(data.Slice(4, 4).Span);
        ParticleData = data.Slice(8, particleDataLen); 
    }

    public Particle[] ReadParticleData()
    {
        var particles = new List<Particle>(ParticleData.Length / Particle.ParticleSaveSize);

        var pos = 0;

        var pdSpan = ParticleData.Span;

        while (pos < ParticleData.Length)
        {
            particles.Add(new Particle(pdSpan.Slice(pos, Particle.ParticleSaveSize), Version));
            pos += Particle.ParticleSaveSize;
        }

        return particles.ToArray();
    }
}