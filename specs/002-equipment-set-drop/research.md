# Research: 套裝系統與裝備掉落機制

**Feature**: 002-equipment-set-drop
**Date**: 2025-12-30
**Status**: Complete

## Research Questions

### RQ-001: 現有裝備系統架構

**Question**: 現有裝備系統如何運作？有哪些模型和服務？

**Findings**:

1. **Item 模型** (`Backend/Models/Item.cs`)
   - ItemType enum: Weapon, Armor, Consumable, Quest, Accessory
   - PropertiesJson 儲存屬性：`{"Atk": 5, "Str": 3}`
   - RequiredLevel, RequiredClass 限制裝備條件

2. **InventoryItem 模型** (`Backend/Models/InventoryItem.cs`)
   - EquipmentSlot enum: None, Weapon, Head, Body, Hands, Feet, Accessory
   - IsEquipped, EquippedSlot 追蹤裝備狀態
   - PlayerId, ItemId 關聯玩家和物品

3. **裝備加成計算** (`Backend/Services/CombatManager.cs:430-481`)
   - GetEquipmentBonusesAsync() 方法
   - 遍歷所有已裝備物品
   - 反序列化 PropertiesJson 為 Dictionary
   - 正規化 key (atk/attack → Atk)
   - 累加所有屬性加成

**Decision**: 沿用現有 PropertiesJson 模式，新增 Quality 和 SetId 欄位

---

### RQ-002: 現有掉落系統架構

**Question**: 現有掉落系統如何運作？

**Findings**:

1. **LootTableEntry 模型** (`Backend/Models/LootTable.cs`)
   - MonsterId → ItemId 對應
   - DropRate: 0-100% 機率
   - MinQuantity, MaxQuantity: 數量範圍

2. **掉落邏輯** (`Backend/Services/CombatManager.cs:278-316`)
   ```csharp
   var lootEntries = await db.LootTableEntries
       .Include(l => l.Item)
       .Where(l => l.MonsterId == monster.Id)
       .ToListAsync();

   foreach (var entry in lootEntries)
   {
       if (_random.NextDouble() * 100 <= entry.DropRate)
       {
           var quantity = _random.Next(entry.MinQuantity, entry.MaxQuantity + 1);
           // 建立 InventoryItem
       }
   }
   ```

3. **現有種子資料** (`Backend/Data/AppDbContext.cs`)
   - 16 個 LootTableEntry
   - Boss 掉落率 100%，普通怪 15-75%
   - 掉落材料、藥水、裝備混合

**Decision**: 在現有掉落邏輯之後新增獨立的裝備掉落判定，不修改 LootTableEntry

---

### RQ-003: 套裝系統最佳實踐

**Question**: RPG 遊戲套裝系統常見的實作模式？

**Findings**:

1. **套裝定義模式**
   - 套裝表 (SetId, Name, Description)
   - 套裝部件關聯 (Item.SetId nullable FK)
   - 套裝加成表 (SetId, RequiredPieces, BonusJson)

2. **加成計算模式**
   - 計算已裝備的同 SetId 物品數量
   - 查詢 SetBonus 表找出所有達標的加成
   - 累加到裝備加成中

3. **常見套裝設計**
   - 2件效果：入門獎勵，易達成
   - 3件效果：中期獎勵
   - 4件/全套效果：完整獎勵，較強

**Decision**: 採用 EquipmentSet + SetBonus 兩表設計，支援靈活的多階段加成

---

### RQ-004: 裝備品質系統實作

**Question**: 如何實作裝備品質和顏色顯示？

**Findings**:

1. **品質等級 (業界標準)**
   - Common (白色) - 70% 普通怪
   - Uncommon (綠色) - 25% 普通怪
   - Rare (藍色) - 4.5% 普通怪
   - Legendary (紫色) - 0.5% 普通怪

2. **品質顏色 (TailwindCSS)**
   - Common: `text-gray-300` (白/灰)
   - Uncommon: `text-green-400` (綠)
   - Rare: `text-blue-400` (藍)
   - Legendary: `text-purple-400` (紫)

3. **前端顯示位置**
   - InventoryPanel.vue 物品名稱
   - 掉落訊息（金色高亮）

**Decision**: 使用 C# enum 定義 ItemQuality，前端用 computed 映射顏色 class

---

### RQ-005: 怪物等級與掉落率關係

**Question**: 如何根據怪物等級決定掉落率？

**Findings**:

1. **現有怪物資料** (`Backend/Data/AppDbContext.cs`)
   - 9 隻怪物，等級 1-15
   - 3 隻 Boss (Lv 5, 10, 15)
   - Monster.IsBoss 標記

2. **設計規格掉落率**
   | 怪物類型 | 掉落率 |
   |---------|--------|
   | Lv 1-5 普通怪 | 0.5% |
   | Lv 6-10 普通怪 | 1% |
   | Lv 11+ 普通怪 | 2% |
   | Boss | 30-100% (個別設定) |

3. **實作方式**
   ```csharp
   double GetEquipmentDropRate(Monster monster)
   {
       if (monster.IsBoss) return monster.EquipmentDropRate ?? 50.0;
       return monster.Level switch
       {
           <= 5 => 0.5,
           <= 10 => 1.0,
           _ => 2.0
       };
   }
   ```

**Decision**: 使用 Monster.Level 和 IsBoss 判斷，Boss 可個別設定 EquipmentDropRate

---

## Technology Decisions Summary

| 項目 | 決定 | 理由 |
|------|------|------|
| 品質儲存 | Item.Quality enum | 簡單直接，方便查詢 |
| 套裝定義 | EquipmentSet 獨立表 | 結構化，支援多套裝 |
| 套裝加成 | SetBonus 獨立表 | 支援多階段加成 |
| 物品套裝關聯 | Item.SetId nullable FK | 允許非套裝物品 |
| 掉落判定 | 現有邏輯後追加 | 不影響現有系統 |
| 品質判定 | 權重隨機 | 符合設計機率 |
| 前端顯示 | TailwindCSS 顏色 class | 一致性，易維護 |

## Alternatives Considered

### 方案 A: 修改 LootTableEntry (已否決)
- 優點：統一掉落邏輯
- 缺點：影響現有資料，需要遷移所有 seed data
- 否決原因：風險太高，不符合「不影響現有系統」原則

### 方案 B: PropertiesJson 存套裝資訊 (已否決)
- 優點：不需新增表
- 缺點：查詢套裝完成度困難，不結構化
- 否決原因：套裝是重要功能，值得專用模型

### 方案 C: 動態計算品質 (已否決)
- 優點：不需儲存品質
- 缺點：同一物品品質可能變動，不符遊戲設計
- 否決原因：品質應該是物品的固有屬性

## Open Questions (Resolved)

1. ✅ Boss 掉落率是否需要個別設定？→ 是，新增 Monster.EquipmentDropRate
2. ✅ 套裝加成是否支援百分比？→ 是，SetBonus.BonusJson 支援 `{"CritRate": 5}`
3. ✅ 前端如何顯示套裝進度？→ 在裝備詳情中顯示 X/Y 件
