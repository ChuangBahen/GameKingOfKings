namespace KingOfKings.Backend.Models;

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Accessory   // 飾品
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
}
