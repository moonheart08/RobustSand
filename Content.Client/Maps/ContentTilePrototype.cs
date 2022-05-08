using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using YamlDotNet.Serialization.Utilities;

namespace Content.Client.Maps;

[Prototype("Tile")]
public sealed class ContentTilePrototype : IPrototype, IInheritingPrototype, ITileDefinition
{
    [DataField("id", required: true)] public string ID { get; }
    public string Name { get; private set; }
    [DataField("parent", customTypeSerializer:typeof(PrototypeIdSerializer<ContentTilePrototype>))]
    public string? Parent { get; private set; }
    
    [DataField("abstract"), NeverPushInheritance] public bool Abstract { get; private set; }
    [DataField("texture")] public string SpriteName { get; }
    [DataField("friction")] public float Friction { get; }
    
    public ushort TileId { get; private set; }
    public string Path => "/Textures/Tiles/";
    
    public void AssignTileId(ushort id)
    {
        TileId = id;
    }

    public void Setup()
    {
        Name = ID;
    }
}