namespace KingOfKings.Backend.DTOs;

/// <summary>
/// 完整角色狀態 DTO
/// </summary>
public class PlayerFullStatsDto
{
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int Level { get; set; }
    public long Exp { get; set; }
    public long ExpRequired { get; set; }
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int CurrentMp { get; set; }
    public int MaxMp { get; set; }
    public CharacterStatsDto Stats { get; set; } = new();
    public EquipmentBonusesDto EquipmentBonuses { get; set; } = new();
}

public class CharacterStatsDto
{
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

public class EquipmentBonusesDto
{
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Str { get; set; }
    public int Dex { get; set; }
    public int Int { get; set; }
    public int Wis { get; set; }
    public int Con { get; set; }
}

/// <summary>
/// 背包資料 DTO
/// </summary>
public class InventoryDataDto
{
    public List<InventoryItemDto> Items { get; set; } = new();
    public int Gold { get; set; }
}

public class InventoryItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsEquipped { get; set; }
    public string EquippedSlot { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, int> Properties { get; set; } = new();
}

/// <summary>
/// 地圖資料 DTO
/// </summary>
public class MapDataDto
{
    public RoomInfoDto CurrentRoom { get; set; } = new();
    public List<ExitInfoDto> Exits { get; set; } = new();
}

public class RoomInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Monsters { get; set; } = new();
}

public class ExitInfoDto
{
    public string Direction { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public bool HasMonsters { get; set; }
}

/// <summary>
/// 技能資料 DTO
/// </summary>
public class SkillsDataDto
{
    public List<SkillDto> LearnedSkills { get; set; } = new();
    public List<SkillDto> LockedSkills { get; set; } = new();
}

public class SkillDto
{
    public string SkillId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int MpCost { get; set; }
    public int RequiredLevel { get; set; }
    public bool IsLearned { get; set; }
}
