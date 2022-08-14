using Content.Shared.Stacks;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype("outerRimSmelterRecipe")]
public sealed class OuterRimSmelterRecipe : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;


    [DataField("stackProto", customTypeSerializer:typeof(PrototypeIdSerializer<StackPrototype>))]
    public string? StackPrototype;
}
