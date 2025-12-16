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
    public string PropertiesJson { get; set; } = "{}"; // e.g. {"Atk": 5}
}
