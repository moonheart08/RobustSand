using System;
using Content.Client.Rendering;
using Robust.Shared.Maths;

namespace Content.Client.Simulation.ParticleKinds.Abstract;

public abstract class MoltenParticle : ParticleImplementation
{
    public abstract bool MeltOrFreeze { get; }
    
    public abstract float DefaultChangePoint { get; }
    
    public abstract ParticleType FailsafeType { get; }
    
    public abstract string Adjective { get; }

    public override Type ParticleAnalyzeControl => typeof(MoltenAnalyzer);

    public override void Update(ref Particle particle, uint id, Vector2i position, Simulation sim)
    {
                
        if (particle.Variable1 is >= (int) ParticleType.End or <= 0)
            particle.Variable1 = (int)FailsafeType; // failsafe in case it got tampered with somehow.

        var meltedType = sim.Implementations[particle.Variable1];
        var changePoint = DefaultChangePoint;
        var melt = meltedType.HighTemperatureConversion;
        if (melt != null && MeltOrFreeze && melt.Value.type == Type)
            changePoint = melt.Value.temperature;
        
        var freeze = meltedType.LowTemperatureConversion;
        if (freeze != null && !MeltOrFreeze && freeze.Value.type == Type)
            changePoint = freeze.Value.temperature;

        if (MeltOrFreeze)
        {
            if (particle.Temperature < changePoint)
            {
                sim.ChangeParticleType(id, position, ref particle, (ParticleType)particle.Variable1);
            }
        }
        else
        {
            if (particle.Temperature >= changePoint)
            {
                sim.ChangeParticleType(id, position, ref particle, (ParticleType)particle.Variable1);
            }
        }
    }

    public override void OnChangedIntoType(ref Particle particle, uint id, Vector2i position, Simulation sim, ParticleType oldType)
    {
        particle.Variable1 = (int) oldType; // Track our type.
    }
}