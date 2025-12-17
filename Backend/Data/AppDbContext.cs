using KingOfKings.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace KingOfKings.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
    public DbSet<WorldRoom> Rooms { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Monster> Monsters { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<MonsterSpawn> MonsterSpawns { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<LootTableEntry> LootTableEntries { get; set; }
    public DbSet<ActiveCombat> ActiveCombats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Owned Type for Stats
        modelBuilder.Entity<PlayerCharacter>()
            .OwnsOne(p => p.Stats);

        // Seed zones (區域)
        modelBuilder.Entity<Zone>().HasData(
            new Zone { Id = 1, Name = "新手村", Description = "適合新手冒險的安全區域", MinLevel = 1, MaxLevel = 5 },
            new Zone { Id = 2, Name = "低語森林", Description = "森林深處潛伏著危險的野獸和哥布林", MinLevel = 5, MaxLevel = 10 },
            new Zone { Id = 3, Name = "廢棄礦坑", Description = "被亡靈佔據的古老礦坑，充滿死亡的氣息", MinLevel = 10, MaxLevel = 15 }
        );

        // Seed initial data (Zone 1: 新手村)
        modelBuilder.Entity<WorldRoom>().HasData(
            // Zone 1: 新手村 (更新出口連接到新區域)
            new WorldRoom { Id = 1, ZoneId = 1, Name = "新手村廣場", Description = "你站在一個寧靜村莊的中心。一座噴泉在旁邊潺潺作響，村民們悠閒地走過。往東可見一片幽暗的森林入口。", ExitsJson = "{\"n\":2,\"e\":6,\"s\":4,\"w\":3}" },
            new WorldRoom { Id = 2, ZoneId = 1, Name = "練武場", Description = "木製人偶排列成行。空氣中迴盪著木劍敲擊的聲音，幾位新手正在練習基本招式。", ExitsJson = "{\"s\":1}" },
            new WorldRoom { Id = 3, ZoneId = 1, Name = "村長的小屋", Description = "一間溫馨的小屋，瀰漫著藥草的香氣。牆上掛滿古老的地圖和冒險者的紀念品。", ExitsJson = "{\"e\":1}" },
            new WorldRoom { Id = 4, ZoneId = 1, Name = "村莊外圍", Description = "村莊的邊緣地帶，青草隨風搖曳。你可以看到一些史萊姆在附近游蕩，偶爾有野鼠竄過。", ExitsJson = "{\"n\":1,\"s\":5}" },
            new WorldRoom { Id = 5, ZoneId = 1, Name = "史萊姆平原", Description = "一片爬滿史萊姆的原野。牠們看起來大多無害，但被激怒時可能會攻擊。往南可見一個廢棄礦坑的入口...", ExitsJson = "{\"n\":4,\"s\":11}" },

            // Zone 2: 低語森林
            new WorldRoom { Id = 6, ZoneId = 2, Name = "低語森林入口", Description = "高聳的樹木遮蔽了大部分陽光，空氣中瀰漫著潮濕的泥土味。你隱約聽到遠處傳來狼嚎聲...", ExitsJson = "{\"w\":1,\"e\":7}" },
            new WorldRoom { Id = 7, ZoneId = 2, Name = "幽暗林道", Description = "蜿蜒的小徑穿過茂密的森林，地上到處可見野狼的腳印。偶爾可見樹幹上哥布林留下的奇怪記號。", ExitsJson = "{\"w\":6,\"e\":8,\"s\":9}" },
            new WorldRoom { Id = 8, ZoneId = 2, Name = "古老橡樹", Description = "一棵巨大的橡樹矗立在森林深處，樹齡看起來有數百年。樹下堆滿了落葉和小動物的骨骸。", ExitsJson = "{\"w\":7}" },
            new WorldRoom { Id = 9, ZoneId = 2, Name = "哥布林營地", Description = "簡陋的帳篷和篝火殘跡顯示這裡是哥布林的聚集地。空氣中飄散著腐臭的氣味，地上散落著啃食過的骨頭。", ExitsJson = "{\"n\":7,\"s\":10}" },
            new WorldRoom { Id = 10, ZoneId = 2, Name = "酋長的巢穴", Description = "一個用樹枝和獸皮搭建的大型棚屋，這裡是哥布林酋長的領地。牆上掛滿了冒險者的裝備作為戰利品。", ExitsJson = "{\"n\":9}" },

            // Zone 3: 廢棄礦坑
            new WorldRoom { Id = 11, ZoneId = 3, Name = "礦坑入口", Description = "一個被木頭支架撐起的礦坑入口，散發著陰冷潮濕的氣息。牆上的火把早已熄滅，只剩下漆黑的通道。", ExitsJson = "{\"n\":5,\"s\":12}" },
            new WorldRoom { Id = 12, ZoneId = 3, Name = "昏暗隧道", Description = "狹窄的隧道向下延伸，兩側是粗糙的岩壁。地上散落著生鏽的採礦工具和不明的白骨。", ExitsJson = "{\"n\":11,\"s\":13,\"e\":14}" },
            new WorldRoom { Id = 13, ZoneId = 3, Name = "崩塌礦井", Description = "這裡曾經發生過嚴重的坍塌，巨石堵住了大部分通道。在碎石堆中，你可以看到骷髏正在四處遊蕩。", ExitsJson = "{\"n\":12}" },
            new WorldRoom { Id = 14, ZoneId = 3, Name = "詭異祭壇", Description = "礦坑深處竟然有一座古老的祭壇，上面刻滿了不祥的符文。空氣中瀰漫著死亡的氣息，殭屍在此徘徊。", ExitsJson = "{\"w\":12,\"n\":15}" },
            new WorldRoom { Id = 15, ZoneId = 3, Name = "死靈法師密室", Description = "一間被黑暗能量籠罩的密室，牆上的燭火發出詭異的綠光。一位身披黑袍的死靈法師正在進行邪惡的儀式...", ExitsJson = "{\"s\":14}" }
        );

        // Seed monster templates (怪物模板)
        modelBuilder.Entity<Monster>().HasData(
            // Zone 1: 新手村
            new Monster { Id = 1, Name = "史萊姆", Level = 1, MaxHp = 30, CurrentHp = 30, Attack = 5, Defense = 2, ExpReward = 10, LocationId = 4, IsBoss = false, Description = "一團軟趴趴的黏液生物，對新手來說是很好的練習對象。" },
            new Monster { Id = 2, Name = "野鼠", Level = 2, MaxHp = 20, CurrentHp = 20, Attack = 8, Defense = 1, ExpReward = 8, LocationId = 4, IsBoss = false, Description = "敏捷的小型齧齒動物，雖然弱小但攻擊迅速。" },
            new Monster { Id = 3, Name = "史萊姆王", Level = 5, MaxHp = 100, CurrentHp = 100, Attack = 12, Defense = 3, ExpReward = 80, LocationId = 5, IsBoss = true, Description = "史萊姆族群的首領，體型巨大且極具威脅性。" },

            // Zone 2: 低語森林
            new Monster { Id = 4, Name = "野狼", Level = 5, MaxHp = 60, CurrentHp = 60, Attack = 12, Defense = 4, ExpReward = 25, LocationId = 7, IsBoss = false, Description = "森林中的掠食者，成群結隊狩獵，牙齒鋒利無比。" },
            new Monster { Id = 5, Name = "哥布林", Level = 6, MaxHp = 50, CurrentHp = 50, Attack = 15, Defense = 3, ExpReward = 30, LocationId = 9, IsBoss = false, Description = "狡猾的小型類人生物，喜歡在森林中埋伏旅人。" },
            new Monster { Id = 6, Name = "哥布林酋長", Level = 10, MaxHp = 200, CurrentHp = 200, Attack = 25, Defense = 8, ExpReward = 150, LocationId = 10, IsBoss = true, Description = "哥布林部落的首領，身披戰甲，揮舞著巨斧，是森林中最危險的存在。" },

            // Zone 3: 廢棄礦坑
            new Monster { Id = 7, Name = "骷髏", Level = 10, MaxHp = 80, CurrentHp = 80, Attack = 18, Defense = 6, ExpReward = 45, LocationId = 12, IsBoss = false, Description = "被黑暗魔法復活的亡者骨骸，永不疲倦地守護著礦坑。" },
            new Monster { Id = 8, Name = "殭屍", Level = 12, MaxHp = 100, CurrentHp = 100, Attack = 20, Defense = 8, ExpReward = 55, LocationId = 13, IsBoss = false, Description = "腐爛的屍體被邪惡力量驅動，行動遲緩但力量驚人。" },
            new Monster { Id = 9, Name = "死靈法師", Level = 15, MaxHp = 300, CurrentHp = 300, Attack = 35, Defense = 10, ExpReward = 250, LocationId = 15, IsBoss = true, Description = "操控亡靈的邪惡法師，是礦坑中所有不死生物的主人。" }
        );

        // Seed items (物品)
        modelBuilder.Entity<Item>().HasData(
            // Zone 1: 新手村物品
            new Item { Id = 1, Name = "史萊姆凝膠", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "黏稠的凝膠，可用於製作藥水。" },
            new Item { Id = 2, Name = "野鼠尾巴", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "野鼠的尾巴，村裡的雜貨店會收購。" },
            new Item { Id = 3, Name = "新手之戒", Type = ItemType.Accessory, PropertiesJson = "{\"Str\":1,\"Int\":1,\"Wis\":1,\"Dex\":1,\"Con\":1}", RequiredLevel = 1, Description = "適合新手冒險者的戒指，提供少量屬性加成。" },
            new Item { Id = 4, Name = "生鏽的劍", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":5}", RequiredLevel = 1, Description = "一把生鏽的舊劍，勉強能用。" },
            new Item { Id = 5, Name = "木杖", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":3,\"Int\":2}", RequiredLevel = 1, Description = "普通的木製法杖，適合初學法師。" },
            new Item { Id = 6, Name = "生命藥水", Type = ItemType.Consumable, PropertiesJson = "{\"HealHp\":30}", RequiredLevel = 1, Description = "恢復 30 點生命值。" },
            new Item { Id = 7, Name = "魔力藥水", Type = ItemType.Consumable, PropertiesJson = "{\"HealMp\":20}", RequiredLevel = 1, Description = "恢復 20 點魔力值。" },

            // Zone 2: 低語森林物品
            new Item { Id = 8, Name = "狼皮", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "粗糙的狼皮，可用於製作皮革裝備。" },
            new Item { Id = 9, Name = "狼牙", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "鋒利的狼牙，常用於製作飾品或武器。" },
            new Item { Id = 10, Name = "生鏽短刀", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":8}", RequiredLevel = 5, Description = "哥布林使用的短刀，雖然生鏽但仍然鋒利。" },
            new Item { Id = 11, Name = "皮革護甲", Type = ItemType.Armor, PropertiesJson = "{\"Def\":5}", RequiredLevel = 5, Description = "基本的皮革護甲，提供不錯的防護。" },
            new Item { Id = 12, Name = "酋長之斧", Type = ItemType.Weapon, PropertiesJson = "{\"Atk\":15,\"Str\":3}", RequiredLevel = 8, RequiredClass = 0, Description = "哥布林酋長的巨斧，只有戰士能夠駕馭。" },

            // Zone 3: 廢棄礦坑物品
            new Item { Id = 13, Name = "骨頭碎片", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "骷髏的殘骸，散發著微弱的魔力。" },
            new Item { Id = 14, Name = "腐肉", Type = ItemType.Quest, PropertiesJson = "{}", RequiredLevel = 1, Description = "腐爛的肉塊，用途不明，或許死靈法師需要它。" },
            new Item { Id = 15, Name = "法師長袍", Type = ItemType.Armor, PropertiesJson = "{\"Def\":3,\"Int\":8,\"Wis\":5}", RequiredLevel = 12, RequiredClass = 1, Description = "死靈法師的長袍，蘊含強大的魔力，只有法師能穿戴。" },
            new Item { Id = 16, Name = "高級生命藥水", Type = ItemType.Consumable, PropertiesJson = "{\"HealHp\":80}", RequiredLevel = 8, Description = "濃縮的生命藥水，恢復 80 點生命值。" },
            new Item { Id = 17, Name = "高級魔力藥水", Type = ItemType.Consumable, PropertiesJson = "{\"HealMp\":50}", RequiredLevel = 8, Description = "濃縮的魔力藥水，恢復 50 點魔力值。" }
        );

        // Seed skills
        modelBuilder.Entity<Skill>().HasData(
            // Warrior skills
            new Skill { Id = 1, SkillId = "bash", Name = "Bash", Description = "A powerful physical strike.", Type = SkillType.Physical, TargetType = SkillTargetType.SingleEnemy, MpCost = 5, Cooldown = 3, BasePower = 20, ScalingStat = "STR", ScalingMultiplier = 1.5, RequiredClass = ClassType.Warrior, RequiredLevel = 1 },
            new Skill { Id = 2, SkillId = "taunt", Name = "Taunt", Description = "Force the enemy to focus on you.", Type = SkillType.Physical, TargetType = SkillTargetType.SingleEnemy, MpCost = 3, Cooldown = 10, BasePower = 0, ScalingStat = "CON", ScalingMultiplier = 0, RequiredClass = ClassType.Warrior, RequiredLevel = 3 },
            new Skill { Id = 3, SkillId = "ironskin", Name = "Iron Skin", Description = "Temporarily increase defense.", Type = SkillType.Physical, TargetType = SkillTargetType.Self, MpCost = 10, Cooldown = 30, BasePower = 0, ScalingStat = "CON", ScalingMultiplier = 0.5, RequiredClass = ClassType.Warrior, RequiredLevel = 5 },
            // Mage skills
            new Skill { Id = 4, SkillId = "fireball", Name = "Fireball", Description = "Hurl a ball of fire at the enemy.", Type = SkillType.Magical, TargetType = SkillTargetType.SingleEnemy, MpCost = 10, Cooldown = 2, BasePower = 25, ScalingStat = "INT", ScalingMultiplier = 2.0, RequiredClass = ClassType.Mage, RequiredLevel = 1 },
            new Skill { Id = 5, SkillId = "icestorm", Name = "Ice Storm", Description = "Unleash a storm of ice on all enemies.", Type = SkillType.Magical, TargetType = SkillTargetType.AllEnemies, MpCost = 25, Cooldown = 10, BasePower = 15, ScalingStat = "INT", ScalingMultiplier = 1.5, RequiredClass = ClassType.Mage, RequiredLevel = 5 },
            new Skill { Id = 6, SkillId = "manashield", Name = "Mana Shield", Description = "Convert damage to MP cost.", Type = SkillType.Magical, TargetType = SkillTargetType.Self, MpCost = 20, Cooldown = 60, BasePower = 0, ScalingStat = "INT", ScalingMultiplier = 0, RequiredClass = ClassType.Mage, RequiredLevel = 3 },
            // Priest skills
            new Skill { Id = 7, SkillId = "heal", Name = "Heal", Description = "Restore HP to yourself.", Type = SkillType.Healing, TargetType = SkillTargetType.Self, MpCost = 8, Cooldown = 3, BasePower = 30, ScalingStat = "WIS", ScalingMultiplier = 2.0, RequiredClass = ClassType.Priest, RequiredLevel = 1 },
            new Skill { Id = 8, SkillId = "bless", Name = "Bless", Description = "Increase STR and DEX temporarily.", Type = SkillType.Healing, TargetType = SkillTargetType.Self, MpCost = 15, Cooldown = 30, BasePower = 0, ScalingStat = "WIS", ScalingMultiplier = 0.3, RequiredClass = ClassType.Priest, RequiredLevel = 3 },
            new Skill { Id = 9, SkillId = "smite", Name = "Smite", Description = "Holy damage that scales with WIS.", Type = SkillType.Magical, TargetType = SkillTargetType.SingleEnemy, MpCost = 12, Cooldown = 4, BasePower = 20, ScalingStat = "WIS", ScalingMultiplier = 1.8, RequiredClass = ClassType.Priest, RequiredLevel = 1 }
        );

        // Seed loot table (掉落表)
        modelBuilder.Entity<LootTableEntry>().HasData(
            // Zone 1: 新手村掉落
            new LootTableEntry { Id = 1, MonsterId = 1, ItemId = 1, DropRate = 50, MinQuantity = 1, MaxQuantity = 2 }, // 史萊姆 -> 史萊姆凝膠
            new LootTableEntry { Id = 2, MonsterId = 2, ItemId = 2, DropRate = 60, MinQuantity = 1, MaxQuantity = 1 }, // 野鼠 -> 野鼠尾巴
            new LootTableEntry { Id = 3, MonsterId = 3, ItemId = 3, DropRate = 100, MinQuantity = 1, MaxQuantity = 1 }, // 史萊姆王 -> 新手之戒
            new LootTableEntry { Id = 4, MonsterId = 1, ItemId = 6, DropRate = 15, MinQuantity = 1, MaxQuantity = 1 }, // 史萊姆 -> 生命藥水
            new LootTableEntry { Id = 5, MonsterId = 2, ItemId = 6, DropRate = 15, MinQuantity = 1, MaxQuantity = 1 }, // 野鼠 -> 生命藥水
            new LootTableEntry { Id = 6, MonsterId = 3, ItemId = 7, DropRate = 30, MinQuantity = 1, MaxQuantity = 2 }, // 史萊姆王 -> 魔力藥水

            // Zone 2: 低語森林掉落
            new LootTableEntry { Id = 7, MonsterId = 4, ItemId = 8, DropRate = 60, MinQuantity = 1, MaxQuantity = 1 },  // 野狼 -> 狼皮
            new LootTableEntry { Id = 8, MonsterId = 4, ItemId = 9, DropRate = 40, MinQuantity = 1, MaxQuantity = 2 },  // 野狼 -> 狼牙
            new LootTableEntry { Id = 9, MonsterId = 5, ItemId = 10, DropRate = 25, MinQuantity = 1, MaxQuantity = 1 }, // 哥布林 -> 生鏽短刀
            new LootTableEntry { Id = 10, MonsterId = 5, ItemId = 11, DropRate = 15, MinQuantity = 1, MaxQuantity = 1 }, // 哥布林 -> 皮革護甲
            new LootTableEntry { Id = 11, MonsterId = 6, ItemId = 12, DropRate = 100, MinQuantity = 1, MaxQuantity = 1 }, // 哥布林酋長 -> 酋長之斧
            new LootTableEntry { Id = 12, MonsterId = 6, ItemId = 16, DropRate = 50, MinQuantity = 1, MaxQuantity = 2 }, // 哥布林酋長 -> 高級生命藥水

            // Zone 3: 廢棄礦坑掉落
            new LootTableEntry { Id = 13, MonsterId = 7, ItemId = 13, DropRate = 70, MinQuantity = 1, MaxQuantity = 3 }, // 骷髏 -> 骨頭碎片
            new LootTableEntry { Id = 14, MonsterId = 8, ItemId = 14, DropRate = 60, MinQuantity = 1, MaxQuantity = 2 }, // 殭屍 -> 腐肉
            new LootTableEntry { Id = 15, MonsterId = 9, ItemId = 15, DropRate = 100, MinQuantity = 1, MaxQuantity = 1 }, // 死靈法師 -> 法師長袍
            new LootTableEntry { Id = 16, MonsterId = 9, ItemId = 17, DropRate = 60, MinQuantity = 1, MaxQuantity = 3 }  // 死靈法師 -> 高級魔力藥水
        );
    }
}
