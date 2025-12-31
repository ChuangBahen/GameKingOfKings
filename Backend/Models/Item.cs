namespace KingOfKings.Backend.Models;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Accessory   // 飾品
}

/// <summary>
/// 裝備品質等級
/// </summary>
public enum ItemQuality
{
    Common = 0,     // 白色 - 普通
    Uncommon = 1,   // 綠色 - 精良
    Rare = 2,       // 藍色 - 稀有
    Legendary = 3   // 紫色 - 傳說
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ItemType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PropertiesJson { get; set; } = "{}"; // e.g. {"Atk": 5}

    /// <summary>
    /// Required level to equip this item.
    /// 裝備此物品所需的等級。
    /// </summary>
    public int RequiredLevel { get; set; } = 1;

    /// <summary>
    /// Required class to equip this item. null = any class.
    /// 裝備此物品所需的職業。null 表示任何職業。
    /// 0 = Warrior, 1 = Mage, 2 = Priest
    /// </summary>
    public int? RequiredClass { get; set; }

    /// <summary>
    /// 裝備品質等級
    /// </summary>
    public ItemQuality Quality { get; set; } = ItemQuality.Common;

    /// <summary>
    /// 所屬套裝 ID (null = 非套裝裝備)
    /// </summary>
    public int? SetId { get; set; }

    /// <summary>
    /// 所屬套裝
    /// </summary>
    public EquipmentSet? EquipmentSet { get; set; }

    /// <summary>
    /// 指定裝備欄位 (null = 非裝備或未指定)
    /// For armor/weapon items, specifies which equipment slot this item occupies
    /// </summary>
    public EquipmentSlot? EquipmentSlot { get; set; }
}
