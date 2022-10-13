using Content.Client.Simulation;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Rendering;

[Virtual]
public class BasicAnalyzer : Label
{
    public Particle particle = new Particle();
    public Simulation.Simulation? sim = null;

    protected override void FrameUpdate(FrameEventArgs args)
    {
        if (sim != null)
        {
            Text = $"{sim.Implementations[(int) particle.Type].Name} ({particle.Temperature - 273.15:F2}C)";
        }
    }
}