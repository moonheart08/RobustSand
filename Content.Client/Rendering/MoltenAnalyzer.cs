using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Rendering;

public sealed class MoltenAnalyzer : BasicAnalyzer
{
    protected override void FrameUpdate(FrameEventArgs args)
    {
        if (sim != null)
        {
            var impl = sim.Implementations[(int) particle.Type];
            if (impl is MoltenParticle moltenImpl)
            {
                Text = $"{moltenImpl.Adjective} {sim.Implementations[(int)particle.Variable1].Name.ToLower()} ({particle.Temperature - 273.15:F2}C)";
            }
            else
            {
                Text = "Not molten???";
            }
        }
    }
}