using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Client.Utility;
using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    private readonly Image<Rgba32> _bufferClear;

    public readonly Image<Rgba32> BaseFrame;

    public readonly Image<Rgba32> LiquidFrame;

    private readonly UIBox2i _bufferFrameBox;
    
    private void DrawNewFrame()
    {
        _bufferClear.Blit(_bufferFrameBox, BaseFrame, Vector2i.Zero);
        _bufferClear.Blit(_bufferFrameBox, LiquidFrame, Vector2i.Zero);
        
        for (var x = 0; x < SimulationConfig.SimWidth; x++)
        {
            for (var y = 0; y < SimulationConfig.SimHeight; y++)
            {
                var pos = new Vector2i(x, y);
                var entry = GetPlayfieldEntry(pos);
                if (entry.Type == ParticleType.None)
                    continue;
                ref var part = ref Particles[entry.Id];
                DrawParticle(pos, ref part, BaseFrame, LiquidFrame);
            }
        }
    }

    private void DrawParticle(Vector2i position, ref Particle particle, Image<Rgba32> newFrame, Image<Rgba32> liquidFrame)
    {
        var impl = Implementations[(int) particle.Type];
        impl.Render(ref particle, out var color);
        if ((impl.ParticleRenderFlags & ParticleRenderFlag.Blob) != 0)
            liquidFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
        if ((impl.ParticleRenderFlags & ParticleRenderFlag.Basic) != 0)
            newFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
    }
}