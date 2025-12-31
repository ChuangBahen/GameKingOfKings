# Data Model: 套裝系統與裝備掉落機制

**Feature**: 002-equipment-set-drop
**Date**: 2025-12-30

## Entity Diagram

```
┌─────────────────┐       ┌─────────────────┐
│  EquipmentSet   │       │    SetBonus     │
├─────────────────┤       ├─────────────────┤
│ Id (PK)         │───┐   │ Id (PK)         │
│ Name            │   │   │ SetId (FK)      │←──┐
│ Description     │   │   │ RequiredPieces  │   │
│ TotalPieces     │   │   │ BonusJson       │   │
└─────────────────┘   │   │ Description     │   │
                      │   └─────────────────┘   │
                      │                         │
                      └─────────────────────────┘

┌─────────────────┐       ┌─────────────────┐
│      Item       │       │    Monster      │
├─────────────────┤       ├─────────────────┤
│ Id (PK)         │       │ Id (PK)         │
│ Name            │       │ Name            │
│ Type            │       │ Level           │
│ Quality (NEW)   │       │ IsBoss          │
│ SetId (FK, NEW) │───────│ EquipDropRate   │ (NEW)
│ PropertiesJson  │       │ DroppableItems  │ (NEW, JSON)
│ ...             │       │ ...             │
└─────────────────┘       └─────────────────┘
```

## New Entities

### EquipmentSet (套裝)

定義一組相關裝備的套裝資訊。

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | PK, Identity | 套裝 ID |
| Name | string | Required, Max 50 | 套裝名稱 (e.g., "史萊姆套裝") |
| Description | string | Max 200 | 套裝描述 |
| TotalPieces | int | Required, >= 2 | 套裝總件數 |

**Relationships**:
- Has many `Item` (via Item.SetId)
- Has many `SetBonus` (via SetBonus.SetId)

**Seed Data**:
```json
[
  { "Id": 1, "Name": "史萊姆套裝", "Description": "新手入門套裝", "TotalPieces": 3 },
  { "Id": 2, "Name": "森林獵人套裝", "Description": "敏捷型套裝", "TotalPieces": 4 },
  { "Id": 3, "Name": "死靈法師套裝", "Description": "魔法型套裝", "TotalPieces": 4 }
]
```

---

### SetBonus (套裝加成)

定義套裝在特定件數門檻的加成效果。

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Id | int | PK, Identity | 加成 ID |
| SetId | int | FK → EquipmentSet | 所屬套裝 |
| RequiredPieces | int | Required, >= 2 | 啟用所需件數 |
| BonusJson | string | Required, JSON | 加成屬性 JSON |
| Description | string | Max 100 | 加成描述 (e.g., "HP+20") |

**BonusJson Format**:
```json
{
  "MaxHp": 20,      // 生命值加成
  "MaxMp": 15,      // 魔力值加成
  "Atk": 10,        // 攻擊加成
  "Def": 5,         // 防禦加成
  "Str": 2,         // 力量加成
  "Dex": 5,         // 敏捷加成
  "Int": 8,         // 智力加成
  "CritRate": 5     // 暴擊率加成 (百分比)
}
```

**Seed Data**:
```json
[
  // 史萊姆套裝
  { "SetId": 1, "RequiredPieces": 2, "BonusJson": "{\"MaxHp\":20}", "Description": "HP+20" },
  { "SetId": 1, "RequiredPieces": 3, "BonusJson": "{\"MaxMp\":15,\"Str\":2,\"Dex\":2,\"Int\":2,\"Wis\":2,\"Con\":2}", "Description": "MP+15, 全屬性+2" },

  // 森林獵人套裝
  { "SetId": 2, "RequiredPieces": 2, "BonusJson": "{\"Atk\":5}", "Description": "攻擊+5" },
  { "SetId": 2, "RequiredPieces": 3, "BonusJson": "{\"Dex\":5}", "Description": "敏捷+5" },
  { "SetId": 2, "RequiredPieces": 4, "BonusJson": "{\"Atk\":10,\"CritRate\":5}", "Description": "攻擊+10, 暴擊+5%" },

  // 死靈法師套裝
  { "SetId": 3, "RequiredPieces": 2, "BonusJson": "{\"Int\":8}", "Description": "智力+8" },
  { "SetId": 3, "RequiredPieces": 3, "BonusJson": "{\"MaxMp\":50}", "Description": "MP+50" },
  { "SetId": 3, "RequiredPieces": 4, "BonusJson": "{\"MagicDamage\":15}", "Description": "魔法傷害+15%" }
]
```

---

## Modified Entities

### Item (新增欄位)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| Quality | ItemQuality | Default: Common | 裝備品質 |
| SetId | int? | FK → EquipmentSet, Nullable | 所屬套裝 (null = 非套裝) |

**ItemQuality Enum**:
```csharp
public enum ItemQuality
{
    Common = 0,     // 白色
    Uncommon = 1,   // 綠色
    Rare = 2,       // 藍色
    Legendary = 3   // 紫色
}
```

**Existing Seed Data Updates**:
- 現有裝備保持 Quality = Common, SetId = null
- 新增套裝裝備時設定 SetId

---

### Monster (新增欄位)

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| EquipmentDropRate | double? | 0-100, Nullable | Boss 專用裝備掉落率 |
| DroppableEquipmentIds | string? | JSON Array, Nullable | 可掉落裝備 ID 清單 |

**DroppableEquipmentIds Format**:
```json
[101, 102, 103]  // 可掉落的裝備 Item.Id 清單
```

**Seed Data Updates**:
- 普通怪: EquipmentDropRate = null (使用等級公式)
- Boss: EquipmentDropRate = 50-100
- DroppableEquipmentIds: 根據怪物主題設定

---

## New DTOs

### EquipmentSetDto

```typescript
interface EquipmentSetDto {
  id: number;
  name: string;
  description: string;
  totalPieces: number;
  ownedPieces: number;          // 玩家已擁有件數
  equippedPieces: number;       // 玩家已裝備件數
  bonuses: SetBonusDto[];       // 所有階段加成
  activeBonuses: SetBonusDto[]; // 已啟用的加成
}
```

### SetBonusDto

```typescript
interface SetBonusDto {
  requiredPieces: number;
  description: string;
  bonuses: Record<string, number>;  // e.g., { "MaxHp": 20 }
  isActive: boolean;
}
```

### InventoryItemDto (修改)

新增欄位:
```typescript
interface InventoryItemDto {
  // ... existing fields ...
  quality: ItemQuality;         // NEW: 品質
  setId: number | null;         // NEW: 套裝 ID
  setName: string | null;       // NEW: 套裝名稱 (方便顯示)
}
```

### ItemQuality Enum (前端)

```typescript
enum ItemQuality {
  Common = 0,
  Uncommon = 1,
  Rare = 2,
  Legendary = 3
}

const QualityColors: Record<ItemQuality, string> = {
  [ItemQuality.Common]: 'text-gray-300',
  [ItemQuality.Uncommon]: 'text-green-400',
  [ItemQuality.Rare]: 'text-blue-400',
  [ItemQuality.Legendary]: 'text-purple-400'
};
```

---

## Database Migration

### Migration: AddEquipmentSetSystem

```csharp
// Up
migrationBuilder.CreateTable("EquipmentSets", ...);
migrationBuilder.CreateTable("SetBonuses", ...);
migrationBuilder.AddColumn<int>("Quality", "Items", defaultValue: 0);
migrationBuilder.AddColumn<int?>("SetId", "Items");
migrationBuilder.AddColumn<double?>("EquipmentDropRate", "Monsters");
migrationBuilder.AddColumn<string?>("DroppableEquipmentIds", "Monsters");
migrationBuilder.CreateIndex("IX_Items_SetId", "Items", "SetId");
migrationBuilder.AddForeignKey("FK_Items_EquipmentSets_SetId", ...);

// Down
migrationBuilder.DropForeignKey("FK_Items_EquipmentSets_SetId", "Items");
migrationBuilder.DropIndex("IX_Items_SetId", "Items");
migrationBuilder.DropColumn("DroppableEquipmentIds", "Monsters");
migrationBuilder.DropColumn("EquipmentDropRate", "Monsters");
migrationBuilder.DropColumn("SetId", "Items");
migrationBuilder.DropColumn("Quality", "Items");
migrationBuilder.DropTable("SetBonuses");
migrationBuilder.DropTable("EquipmentSets");
```

---

## Validation Rules

### EquipmentSet
- Name: 不可空白，2-50 字元
- TotalPieces: >= 2

### SetBonus
- SetId: 必須存在對應的 EquipmentSet
- RequiredPieces: >= 2 且 <= EquipmentSet.TotalPieces
- BonusJson: 必須是有效 JSON

### Item.Quality
- 必須是有效的 ItemQuality enum 值

### Item.SetId
- 如果非 null，必須存在對應的 EquipmentSet
- 只有 ItemType = Weapon, Armor, Accessory 可以有 SetId

### Monster.EquipmentDropRate
- 如果非 null，必須在 0-100 之間
- 通常只有 IsBoss = true 的怪物會設定

### Monster.DroppableEquipmentIds
- 如果非 null，必須是有效的 JSON 陣列
- 陣列中的每個 ID 必須對應存在的 Item
