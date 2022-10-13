using System;
using Content.Client.Rendering;
using Content.Client.Simulation.ParticleKinds.Abstract;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Client.Simulation.ParticleKinds.Gas;

[Particle]
public sealed class Fire : ParticleImplementation
{
    [Dependency] private readonly IRobustRandom _random = default!;
    
    protected override ParticleType PType => ParticleType.Fire;
    protected override string PName => "Fire";
    protected override string PDescription => "It burns, it burns!";
    protected override byte PWeight => 15;
    protected override Color PColor => Color.Firebrick;
    protected override float PRateOfGravity => -base.PRateOfGravity / 3;
    protected override float PDiffusionRate => 0.05f;
    protected override float PMaximumVelocity => 1.4f;
    protected override float PSpecificHeat => 2.85f;
    protected override ParticleMovementFlag PMovementFlags => ParticleMovementFlag.Liquid;
    protected override ParticlePropertyFlag PPropertyFlags => ParticlePropertyFlag.Gas;
    protected override ParticleRenderFlag PParticleRenderFlags => ParticleRenderFlag.Blob;

    public const int FireLifespan = 30 * 5;

    private readonly Color[] _fireColors = {Color.FromHex("#000000"), Color.FromHex("#60300F"), Color.FromHex("#DFBF6F"), Color.FromHex("#AF9F0F") };

    private readonly float[] _firePoints = { 0.0f, 0.3f, 0.8f, 1.0f };

    private readonly Color[] _fireGradient;

    public Fire()
    {
        _fireGradient = RenderHelpers.GenerateGradient(_firePoints, _fireColors, FireLifespan+50);
    }

    public override bool OnSpawn(ref Particle particle)
    {
        particle.Variable1 = _random.Next(FireLifespan-30, FireLifespan+30);
        particle.Temperature = 540f;
        return true;
    }
    
    public override void OnChangedIntoType(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType oldType)
    {
        OnSpawn(ref particle); // Do the usual setup.
    }

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
        if (particle.Variable1 <= 0)
        {
            sim.ChangeParticleType(id, position, ref particle, ParticleType.Smoke);
            return;
        }

        particle.Variable1--;
        
        for (var relX = -1; relX <= 1; relX++)
        {
            for (var relY = -1; relY <= 1; relY++)
            {
                var offsPos = position + new Vector2i(relX, relY);
                
                if (!sim.SimulationBounds.Contains(offsPos))
                    continue;

                var entry = sim.GetPlayfieldEntry(offsPos);
                
                if (entry.Type == ParticleType.None)
                    continue;

                sim.Implementations[(int) entry.Type].OnBurned(ref sim.Particles[entry.Id], ref particle, entry.Id, id,
                    offsPos, position, sim);
                
                if (particle.Type != Type)
                    return; // We can't burn things if we're not the correct type.
            }
        }
    }
    
    public override void Render(ref Particle particle, out Color color)
    {
        var colorIdx = Math.Clamp(particle.Variable1, 0, FireLifespan+49);
        color = _fireGradient[colorIdx];
    }
}