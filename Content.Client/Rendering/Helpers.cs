using Robust.Shared.Maths;

namespace Content.Client.Rendering;

public static class Helpers
{
    public static Color[] GenerateGradient(float[] points, Color[] colors, int size)
    {
        var output = new Color[size];

        var i = 0;
        var j = 1;
        var curPoint = points[i];
        var nextPoint = points[j];
        for (var idx = 0; idx < size; idx++)
        {
            var idxClamped = (float)idx / size;
            if(idxClamped > nextPoint && j+1 < points.Length)
            {
                curPoint = points[++i];
                nextPoint = points[++j];
            }
            var ccpos = idxClamped - curPoint;
            var cccpos = ccpos / (nextPoint - curPoint);
            if(cccpos > 1.0f) 
                cccpos = 1.0f;
            output[idx].R = (colors[i].R*(1.0f-cccpos) + colors[j].R)*cccpos;
            output[idx].G = (colors[i].G*(1.0f-cccpos) + colors[j].G)*cccpos;
            output[idx].B = (colors[i].B*(1.0f-cccpos) + colors[j].B)*cccpos;
            output[idx].A = 1.0f;
        }
        return output;
    }
}