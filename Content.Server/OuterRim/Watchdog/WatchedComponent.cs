namespace Content.Server.OuterRim.Watchdog;

/// <summary>
/// This is used for objects being watched by a watchdog.
/// </summary>
[RegisterComponent]
public sealed class WatchedComponent : Component
{
    public EntityUid Watcher = EntityUid.Invalid;
}
