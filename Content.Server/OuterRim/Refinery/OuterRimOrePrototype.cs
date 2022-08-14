using Content.Shared.Materials;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Utility;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype("outerRimOre")]
public sealed class OuterRimOrePrototype : IPrototype, IInheritingPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name", required: true)]
    public string Name { get; } = default!;

    [ParentDataField(typeof(AbstractPrototypeIdArraySerializer<OuterRimOrePrototype>))]
    public string[]? Parents { get; }

    /// <inheritdoc/>
    [NeverPushInheritance]
    [AbstractDataField]
    public bool Abstract { get; }

    [ViewVariables]
    [DataField("materials", customTypeSerializer: typeof(PrototypeIdDictionarySerializer<int, MaterialPrototype>), required: true)]
    [AlwaysPushInheritance]
    // ReSharper disable once CollectionNeverUpdated.Local
    public readonly Dictionary<string, int> Materials = new();

    [DataField("oreSprite", required: true)]
    public readonly SpriteSpecifier OreSprite = SpriteSpecifier.Invalid;
}
