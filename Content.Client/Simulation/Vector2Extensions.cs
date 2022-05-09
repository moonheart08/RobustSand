using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Client.Simulation;

public static class Vector2Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i RoundedI(this Vector2 inp)
    {
        return inp.Rounded().Floored();
    }

    public static Vector2 ClampMagnitude(this Vector2 inp, float magnitude)
    {
        var length = inp.Length;
        
        if (length <= magnitude) return inp;
        
        var mag = (float)Math.Sqrt(length);
        return inp.Normalized * magnitude;
    }
}