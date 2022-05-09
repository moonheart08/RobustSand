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
        var liquidFrame = _bufferClear.Clone();

        for (var x = 0; x < SimulationConfig.SimWidth; x++)
        {
            for (var y = 0; y < SimulationConfig.SimHeight; y++)
            {
                var pos = new Vector2i(x, y);
                var entry = _simSys.Simulation.GetPlayfieldEntry(pos);
                if (entry.Type == ParticleType.NONE)
                    continue;
                ref var part = ref _simSys.Simulation.Particles[entry.Id];
                DrawParticle(pos, ref part, ref newFrame, ref liquidFrame);
            }
        }
        
        _renderBuffer.SetSubImage(Vector2i.Zero, newFrame);
        
        _liquidBuffer.SetSubImage(Vector2i.Zero, liquidFrame);
    }

    private void DrawParticle(Vector2i position, ref Particle particle, ref Image<Rgba32> newFrame, ref Image<Rgba32> liquidFrame)
    {
        var impl = _simSys.Simulation.Implementations[(int) particle.Type];
        impl.Render(ref particle, out var color);
        if ((impl.ParticleRenderFlags & ParticleRenderFlag.Blob) != 0)
            liquidFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
        else
            newFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
    }
}