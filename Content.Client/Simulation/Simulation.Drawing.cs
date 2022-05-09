using System;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;

namespace Content.Client.Simulation;

public sealed partial class Simulation
{
    public uint DrawingRadius = 2;
    
    public void Draw(Vector2i start, Vector2i end, ParticleType placing)
    {
        DrawLine(start, end, placing);
    }

    public void DrawLine(Vector2i start, Vector2i end, ParticleType placing)
    {
        var acc = start;
        var dx = Math.Abs(end.X - start.X);
        var sx = start.X < end.X ? 1 : -1;
        var dy = -Math.Abs(end.Y - start.Y);
        var sy = start.Y < end.Y ? 1 : -1;
        var error = dx + dy;

        while (true)
        {
            DrawBrush(acc, placing);
            if (acc.X == end.X && acc.Y == end.Y) break;
            var e2 = 2 * error;
            if (e2 >= dy)
            {
                if (acc.X == end.X) break;
                error += dy;
                acc.X += sx;
            }
            if (e2 <= dx)
            {
                if (acc.Y == end.Y) break;
                error += dx;
                acc.Y += sy;
            }
        }
    }

    public void DrawBrush(Vector2i pos, ParticleType placing)
    {
        var drawAdj = new Vector2i((int)DrawingRadius, (int)DrawingRadius);
        DrawRect(new Box2i(pos - drawAdj, pos + drawAdj), placing);
    }

    public void DrawRect(Box2i rect, ParticleType placing)
    {
        for (var x = rect.BottomLeft.X; x < rect.TopRight.X; x++)
        {
            for (var y = rect.BottomLeft.Y; y < rect.TopRight.Y; y++)
            {
                DrawPixel(new Vector2i(x, y), placing);
            }
        }
    }
    
    public void DrawPixel(Vector2i pos, ParticleType placing)
    {
        if (!SimulationBounds.Contains(pos))
            return;
        
        var entry = GetPlayfieldEntry(pos);
        if (placing == ParticleType.NONE)
        {
            if (entry.Type == ParticleType.NONE)
                return;
            DeleteParticle(entry.Id, pos, ref Particles[entry.Id]);
        }

        if (entry.Type != ParticleType.NONE)
        {
            Implementations[(int)entry.Type].DrawnOn(ref Particles[entry.Id], entry.Id, pos, this, placing);
            return;
        }

        TrySpawnParticle(pos, placing, out _);
    }

    public void Clear()
    {
        for (uint i = 0; i < _lastActiveParticle; i++)
        {
            ref var part = ref Particles[i];
            if (part.Type != ParticleType.NONE)
                DeleteParticle(i, part.Position.RoundedI(), ref part);
        }
    }
}