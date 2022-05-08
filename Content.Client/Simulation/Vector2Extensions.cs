using Robust.Shared.Maths;

namespace Content.Client.Simulation;

public static class Vector2Extensions
{
    public static Vector2i RoundedI(this Vector2 inp)
    {
        return inp.Rounded().Floored();
    }
}