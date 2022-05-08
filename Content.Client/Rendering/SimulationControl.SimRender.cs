using Content.Client.Simulation;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Robust.Shared.Maths.Color;

namespace Content.Client.Rendering;

public sealed partial class SimulationControl
{
    private Image<Rgba32> _bufferClear;
    
    private void DrawNewFrame()
    {
        var newFrame = _bufferClear.Clone();

        for (var x = 0; x < SimulationConfig.SimWidth; x++)
        {
            for (var y = 0; y < SimulationConfig.SimHeight; y++)
            {
                var pos = new Vector2i(x, y);
                var entry = Simulation.GetPlayfieldEntry(pos);
                if (entry.Type == ParticleType.NONE)
                    continue;
                ref var part = ref Simulation.Particles[entry.Id];
                DrawParticle(pos, ref part, ref newFrame);
            }
        }
        _renderBuffer.SetSubImage(Vector2i.Zero, newFrame);
    }

    private void DrawParticle(Vector2i position, ref Particle particle, ref Image<Rgba32> newFrame)
    {
        Simulation.Implementations[(int) particle.Type].Render(ref particle, out var color);
        newFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
    }
}