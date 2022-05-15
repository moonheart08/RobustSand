using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    private void ProcessParticleTemperature(uint id, ref Particle part)
    {
        var pos = part.Position.RoundedI();
        var impl = Implementations[(int) part.Type];
        // TODO: This has an up-left bias! Need to work that out somehow without tanking perf.
        for (var relX = -1; relX <= 1; relX++)
        {
            for (var relY = -1; relY <= 1; relY++)
            {
                var offsPos = pos + new Vector2i(relX, relY);
                
                if (!SimulationBounds.Contains(offsPos))
                    continue;

                var entry = GetPlayfieldEntry(offsPos);
                
                if (entry.Type == ParticleType.None)
                    continue;

                ref var otherPart = ref Particles[entry.Id];

                var delta = part.Temperature - otherPart.Temperature;
                var heatCap = impl.SpecificHeat; // equivalent as all particles are assumed to be 1kg exactly.
                var otherHeatCap = Implementations[(int) otherPart.Type].SpecificHeat;
                if (heatCap <= 0.0f || otherHeatCap <= 0.0f)
                    continue;

                var heat = delta * (heatCap * otherHeatCap / (heatCap + otherHeatCap));
                part.Temperature = MathF.Abs(MathF.Max(part.Temperature - heat / heatCap, 0.0f));
                otherPart.Temperature = MathF.Abs(MathF.Max(otherPart.Temperature + heat / otherHeatCap, 0.0f));
            }
        }

        if (impl.LowTemperatureConversion is not null &&
            part.Temperature < impl.LowTemperatureConversion.Value.temperature)
        {
            ChangeParticleType(id, pos, ref part, impl.LowTemperatureConversion.Value.type);
        }
        if (impl.HighTemperatureConversion is not null &&
            part.Temperature >= impl.HighTemperatureConversion.Value.temperature)
        {
            ChangeParticleType(id, pos, ref part, impl.HighTemperatureConversion.Value.type);
        }
    }
}