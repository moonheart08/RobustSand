using System;
using Content.Client.Simulation;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Client.Graphics;
using Robust.Client.Utility;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Robust.Shared.Maths.Color;

namespace Content.Client.Rendering;

public sealed partial class SimulationControl
{
    private OwnedTexture _renderBuffer = default!;

    private OwnedTexture _liquidBuffer = default!;

    private OwnedTexture _fireTexture = default!;
    
    private Image<Rgba32> _bufferClear = default!;

    private Image<Rgba32> _newFrame = default!;

    private Image<Rgba32> _liquidFrame = default!;

    private UIBox2i _bufferFrameBox;

    private ShaderInstance _fireShader;


    private void InitializeSimRender()
    {
        _renderBuffer = _clyde.CreateBlankTexture<Rgba32>(
            new Vector2i((int) SimulationConfig.SimWidth, (int) SimulationConfig.SimHeight), "simbuffer",
            TextureLoadParameters.Default);
        _liquidBuffer = _clyde.CreateBlankTexture<Rgba32>(_renderBuffer.Size, "simbuffer");
        _fireTexture =
            _clyde.CreateBlankTexture<Rgba32>(new Vector2i(12, 12), "fireTex", TextureLoadParameters.Default);
        BuildFireTexture();
        _bufferClear = new Image<Rgba32>(_renderBuffer.Width, _renderBuffer.Height, new Rgba32(0, 0, 0, 0));
        _newFrame = _bufferClear.Clone();
        _liquidFrame = _bufferClear.Clone();
        _bufferFrameBox = UIBox2i.FromDimensions(Vector2i.Zero, _renderBuffer.Size);
        _fireShader = _prototypeManager.Index<ShaderPrototype>("fire").Instance();
    }
    
    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);
        var rect = UIBox2
            .FromDimensions(Vector2.Zero, new Vector2(SimulationConfig.SimWidth, SimulationConfig.SimHeight) * (UIScale < 1 ? 1.0f : UIScale));
        
        handle.DrawTextureRect(_renderBuffer, rect);
        for (var relX = -2; relX < 3; relX++)
        {
            for (var relY = -2; relY < 3; relY++)
            {
                var amt = (float)(Math.Exp(-0.3 * (relX * relX + relY * relY)) / 5.0);

                handle.DrawTextureRect(_liquidBuffer, rect.Translated(new Vector2(relX, relY)), new Color(1.0f, 1.0f, 1.0f, amt));
            }
        }
        // Fire effects use quads and prayers.
        DrawFire(handle);
        handle.DrawCircle(MousePosition, _simSys.Simulation.DrawingRadius, Color.White, false);
        MinSize = rect.Size;
    }

    private void DrawNewFrame()
    {
        _bufferClear.Blit(_bufferFrameBox, _newFrame, Vector2i.Zero);
        _bufferClear.Blit(_bufferFrameBox, _liquidFrame, Vector2i.Zero);

        for (var x = 0; x < SimulationConfig.SimWidth; x++)
        {
            for (var y = 0; y < SimulationConfig.SimHeight; y++)
            {
                var pos = new Vector2i(x, y);
                var entry = _simSys.Simulation.GetPlayfieldEntry(pos);
                if (entry.Type == ParticleType.NONE)
                    continue;
                ref var part = ref _simSys.Simulation.Particles[entry.Id];
                DrawParticle(pos, ref part, ref _newFrame, ref _liquidFrame);
            }
        }

        _renderBuffer.SetSubImage(Vector2i.Zero, _newFrame);
        
        _liquidBuffer.SetSubImage(Vector2i.Zero, _liquidFrame);
    }
    
    private void DrawFire(DrawingHandleScreen handle)
    {
        var fireQuadMem = new Memory<float>(new float[4*2]);
        var fireQuad = fireQuadMem.Span;
        
        //handle.UseShader(_fireShader);
        
        for (var i = 0; i < _simSys.Simulation.Particles.Length; i++)
        {
            ref var part = ref _simSys.Simulation.Particles[i];
            var entry = _simSys.Simulation.GetPlayfieldEntry(part.Position.RoundedI());
            if (entry.Type == ParticleType.NONE)
                continue;
            var impl = _simSys.Simulation.Implementations[(int) entry.Type];
            if ((impl.ParticleRenderFlags & ParticleRenderFlag.Fire) == 0)
                continue;
            impl.Render(ref part, out var color);
            handle.DrawTextureRect(_fireTexture, UIBox2i.FromDimensions(part.Position.RoundedI(), new Vector2i(12, 12)).Translated(-(new Vector2i(12, 12) / 2)),
                color);
        }
        
        handle.UseShader(null);
    }
    
    private void DrawParticle(Vector2i position, ref Particle particle, ref Image<Rgba32> newFrame, ref Image<Rgba32> liquidFrame)
    {
        var impl = _simSys.Simulation.Implementations[(int) particle.Type];
        impl.Render(ref particle, out var color);
        var flags = impl.ParticleRenderFlags;
        if ((flags & ParticleRenderFlag.Blob) != 0)
            liquidFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
        if ((flags & ParticleRenderFlag.None) == 0)
            newFrame[position.X, position.Y] = new Rgba32(color.R, color.G, color.B, color.A);
    }

    private void BuildFireTexture()
    {
        var img = new Image<Rgba32>(12, 12, new Rgba32(0, 0, 0, 0));
        for (var suvx = -6; suvx < 6; suvx++)
        {
            for (var suvy = -6; suvy < 6; suvy++)
            {
                img[suvx + 6, suvy + 6] = new Rgba32(1.0f,1.0f,1.0f, (float)(Math.Exp(-0.05 * (suvx * suvx + suvy * suvy)) / 10.0));
            }
        }
        
        _fireTexture.SetSubImage(Vector2i.Zero, img);
    }

}