using Content.Server.Chat.Systems;
using Content.Server.Construction;
using Content.Server.IdentityManagement;
using Content.Server.Tools.Components;
using Content.Shared.Construction.Components;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Timing;

namespace Content.Server.OuterRim.Watchdog;

/// <summary>
/// This handles...
/// </summary>
public sealed class WatchdogSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chatSystem = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<WatchdogComponent, MapInitEvent>(OnMapInit);
        // Doesn't subscribe to UserAnchoredEvent on purpose.
        SubscribeLocalEvent<WatchedComponent, UserUnanchoredEvent>(OnUserUnanchored);
        SubscribeLocalEvent<WatchedComponent, AttackedEvent>(OnAttacked);
        SubscribeLocalEvent<WatchedComponent, EntParentChangedMessage>(OnParentChanged);
    }

    private void OnParentChanged(EntityUid uid, WatchedComponent component, ref EntParentChangedMessage args)
    {
        if (args.Transform.GridUid != component.Watcher)
            QueueDel(uid); // As a last stance against theft, outright delete stolen objects.
    }

    public override void Update(float frameTime)
    {
        foreach (var watchdog in EntityQuery<WatchdogComponent>())
        {
            var toBlacklist = new List<EntityUid>();
            foreach (var (target, time) in watchdog.TargetsInGrace)
            {
                if (_gameTiming.CurTime - time >= watchdog.GracePeriod)
                {
                    toBlacklist.Add(target);
                }
            }

            foreach (var target in toBlacklist)
            {
                Blacklist(watchdog, target);
            }
        }
    }

    private void OnAttacked(EntityUid uid, WatchedComponent component, AttackedEvent args)
    {
        AngerAt(component.Watcher, args.User, component);
    }

    private void OnUserUnanchored(EntityUid uid, WatchedComponent component, UserUnanchoredEvent args)
    {
        AngerAt(component.Watcher, args.User, component);
    }

    private void OnMapInit(EntityUid uid, WatchdogComponent component, MapInitEvent args)
    {
        WatchRecursive(uid, uid);
    }

    private void WatchRecursive(EntityUid uid, EntityUid watcher)
    {
        var xform = Transform(uid);
        foreach (var ent in xform.ChildEntities)
        {
            if (HasComp<ItemComponent>(ent) && !HasComp<AnchorableComponent>(ent))
                continue;

            var watched = EnsureComp<WatchedComponent>(ent);
            watched.Watcher = uid;
            WatchRecursive(ent, watcher);
        }
    }

    private void AngerAt(EntityUid watched, EntityUid target, WatchedComponent comp)
    {
        var watchdog = Comp<WatchdogComponent>(comp.Watcher);
        if (watchdog.TargetEntities.Contains(target))
            return;

        if (watchdog.TargetsInGrace.ContainsKey(target))
        {
            Blacklist(watchdog, target);
            return;
        }

        if (!watchdog.WarnedEntities.ContainsKey(target))
            watchdog.WarnedEntities[target] = 0;

        watchdog.WarnedEntities[target]++;

        if (watchdog.WarnedEntities[target] > watchdog.Warnings)
        {
            EnterGrace(watchdog, target);
        }
        else
        {
            Warn(watchdog, target);
        }
    }

    private void Warn(WatchdogComponent watchdog, EntityUid target)
    {
        _chatSystem.DispatchGlobalAnnouncement(Loc.GetString(watchdog.WarningMessage,
            ("name", Identity.Entity(target, EntityManager))),
            Loc.GetString(watchdog.AnnouncementSpeaker));
    }

    private void EnterGrace(WatchdogComponent watchdog, EntityUid target)
    {
        watchdog.WarnedEntities.Remove(target);
        watchdog.TargetsInGrace.Add(target, _gameTiming.CurTime);
        _chatSystem.DispatchGlobalAnnouncement(Loc.GetString(watchdog.EnterGraceMessage,
                ("name", Identity.Entity(target, EntityManager)),
                ("grace", $"{watchdog.GracePeriod.TotalMinutes:F1} minutes")),
            Loc.GetString(watchdog.AnnouncementSpeaker));
    }

    private void Blacklist(WatchdogComponent watchdog, EntityUid target)
    {
        watchdog.TargetsInGrace.Remove(target);
        watchdog.TargetEntities.Add(target);
        var xform = Transform(target);
        if (xform.GridUid == watchdog.Owner)
        {
            _chatSystem.DispatchGlobalAnnouncement(
                Loc.GetString(watchdog.AttackMessage, ("name", Identity.Entity(target, EntityManager))),
                Loc.GetString(watchdog.AnnouncementSpeaker));
        }
    }
}
