using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Maps;

[Prototype("GameMap")]
public sealed class GameMapPrototype : IPrototype
{
    [DataField("id")]
    public string ID { get; }
    
    [DataField("path")]
    public ResourcePath MapPath { get; }
    
    [DataField("ambientLighting")]
    public Color AmbientLighting { get; }
}