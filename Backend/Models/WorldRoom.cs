namespace KingOfKings.Backend.Models;

public class WorldRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // JSON string for exits: { "n": 2, "e": 3 }
    public string ExitsJson { get; set; } = "{}";

    /// <summary>
    /// The zone this room belongs to.
    /// 此房間所屬的區域。
    /// </summary>
    public int ZoneId { get; set; } = 1;
}
