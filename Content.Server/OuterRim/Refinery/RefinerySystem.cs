using Content.Server.Stack;
using Content.Shared.Materials;
using Content.Shared.Stacks;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This handles...
/// </summary>
public sealed class RefinerySystem : EntitySystem
{
    [Dependency] private readonly StackSystem _stackSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IComponentFactory _componentFactory = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {

    }

    public void MaterializeOre(EntityUid uid, OuterRimOreComponent? oreComp, float efficiency, Dictionary<string, int>? leftover = null, EntityCoordinates? spawnLocation = null)
    {
        if (!Resolve(uid, ref oreComp))
            return;

        spawnLocation ??= Transform(uid).Coordinates;

        if (!_prototypeManager.TryIndex<OuterRimOrePrototype>(oreComp.OreKind, out var oreKind))
            return;

        foreach (var (kind, amount) in oreKind.Materials)
        {
            if (!_prototypeManager.TryIndex<MaterialPrototype>(kind, out var mat))
                continue;

            if (mat.StackId is null) // Can't make a non-existent stack.
                continue;

            if (!_prototypeManager.TryIndex<StackPrototype>(mat.StackId, out var stack))
                continue;

            var stackCount = (int)(amount * efficiency) / mat.MaterialPerStackUnit;
            var remainder = (int)(amount * efficiency) % mat.MaterialPerStackUnit;
            if (leftover is not null)
                leftover[kind] = remainder;

            if (GetStackMax(stack) is not { } max)
                return;

            while (stackCount > 0)
            {
                var amountToSpawn = Math.Min(max, stackCount);
                stackCount -= amountToSpawn;
                var ent = Spawn(stack.Spawn, spawnLocation.Value);
                _stackSystem.SetCount(ent, amountToSpawn);
            }
        }
    }

    // TODO: This does evil shit, fix it upstream.
    private int? GetStackMax(StackPrototype proto)
    {
        if (!_prototypeManager.TryIndex<EntityPrototype>(proto.Spawn, out var entProto))
            return null;

        var stackCompName = _componentFactory.GetComponentName(typeof(StackComponent));
        var comp = (StackComponent)entProto.Components[stackCompName].Component;
        return comp.MaxCount;
    }
}
