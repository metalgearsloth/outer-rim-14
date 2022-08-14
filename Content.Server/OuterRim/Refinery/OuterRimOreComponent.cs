using Content.Shared.Materials;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is used for storing information about ore.
/// </summary>
[RegisterComponent]
public sealed class OuterRimOreComponent : Component
{
    [DataField("oreKind", customTypeSerializer: typeof(PrototypeIdSerializer<OuterRimOrePrototype>), required: true)]
    public string OreKind = default!;
}
