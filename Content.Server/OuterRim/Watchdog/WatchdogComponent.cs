namespace Content.Server.OuterRim.Watchdog;

/// <summary>
/// This is used for allowing structures to be angry at people.
/// </summary>
[RegisterComponent]
public sealed class WatchdogComponent : Component
{
    /// <summary>
    /// The watchdog grace period.
    /// </summary>
    public TimeSpan GracePeriod = TimeSpan.FromMinutes(2);
    /// <summary>
    /// Entities that have been warned, and how many times.
    /// </summary>
    public Dictionary<EntityUid, int> WarnedEntities = new();
    /// <summary>
    /// Entities in their grace period to leave before they're put on the target list.
    /// </summary>
    public Dictionary<EntityUid, TimeSpan> TargetsInGrace = new();
    /// <summary>
    /// Entities that're on the target list.
    /// </summary>
    public List<EntityUid> TargetEntities = new();

    [DataField("announcementSpeaker")] public string AnnouncementSpeaker = "laros-name";

    [DataField("warningMessage")] public string WarningMessage = "laros-watchdog-warn-entity";
    [DataField("enterGraceMessage")] public string EnterGraceMessage = "laros-watchdog-enter-grace-entity";
    [DataField("attackMessage")] public string AttackMessage = "laros-watchdog-attack-entity";

    public int Warnings = 2;
}
