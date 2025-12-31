# Quickstart: 套裝系統與裝備掉落機制

**Feature**: 002-equipment-set-drop
**Date**: 2025-12-30

## 快速開始

### 1. 環境準備

```bash
# 後端
cd Backend
dotnet restore
dotnet build

# 前端
cd Frontend
npm install
```

### 2. 資料庫遷移

```bash
cd Backend

# 建立遷移 (如果尚未建立)
dotnet ef migrations add AddEquipmentSetSystem

# 套用遷移
dotnet ef database update
```

### 3. 啟動服務

```bash
# Terminal 1: 後端
cd Backend
dotnet run

# Terminal 2: 前端
cd Frontend
npm run dev
```

### 4. 測試網址

- 前端: http://localhost:5173
- 後端 API: http://localhost:5000

---

## 功能驗證

### 驗證 1: 裝備品質顏色

1. 登入遊戲
2. 打開背包 (Inventory Panel)
3. 查看裝備名稱顏色:
   - Common → 白色/灰色
   - Uncommon → 綠色
   - Rare → 藍色
   - Legendary → 紫色

**預期結果**: 不同品質裝備顯示對應顏色

---

### 驗證 2: 裝備掉落

1. 找到普通怪物 (Lv 1-5)
2. 擊殺約 200 隻 (期望 1 件掉落, 0.5% 機率)
3. 或找到 Boss 擊殺
4. 觀察掉落訊息

**預期結果**:
- 普通怪: 極低機率掉落裝備 (0.5-2%)
- Boss: 較高機率掉落裝備 (30-100%)
- 掉落訊息使用金色文字顯示

---

### 驗證 3: 套裝資訊顯示

1. 獲得一件套裝裝備 (e.g., 史萊姆套裝部件)
2. 查看裝備詳情
3. 確認顯示套裝名稱和資訊

**預期結果**:
- 顯示「這是 XXX 套裝的一部分」
- 顯示套裝所有部件清單
- 顯示已收集件數 (X/Y)

---

### 驗證 4: 套裝加成啟用

1. 獲得並裝備同套裝 2 件裝備
2. 查看角色屬性面板
3. 確認套裝加成生效

**預期結果**:
- 史萊姆套裝 2 件: HP+20
- 屬性面板顯示套裝效果區塊

---

### 驗證 5: 套裝加成降階

1. 已裝備同套裝 2 件以上
2. 脫下其中一件
3. 觀察屬性變化

**預期結果**:
- 套裝加成立即重新計算
- 如果低於 2 件，加成消失

---

## 測試指令

### 手動給予裝備 (GM 指令)

```
/give item <item_id> [quantity]
```

**套裝裝備 ID (種子資料)**:
- 史萊姆套裝: 101, 102, 103
- 森林獵人套裝: 104, 105, 106, 107
- 死靈法師套裝: 108, 109, 110, 111

### 強制掉落測試

```
/drop equipment <monster_id>
```

---

## 常見問題

### Q: 裝備沒有顯示品質顏色？

**A**: 確認以下事項:
1. 資料庫遷移已套用
2. InventoryItemDto 包含 quality 欄位
3. 前端 InventoryPanel.vue 已更新

### Q: 套裝加成沒有生效？

**A**: 確認以下事項:
1. 裝備的 SetId 正確設定
2. SetBonus 種子資料已載入
3. GetEquipmentBonusesAsync 包含套裝加成邏輯

### Q: 掉落率不正確？

**A**: 確認以下事項:
1. Monster.Level 正確設定
2. Monster.IsBoss 正確標記
3. 掉落率公式邏輯正確:
   - Lv 1-5: 0.5%
   - Lv 6-10: 1%
   - Lv 11+: 2%
   - Boss: EquipmentDropRate 或預設 50%

---

## 開發注意事項

1. **不要修改現有 LootTableEntry**
   - 裝備掉落是獨立邏輯
   - 在現有掉落之後執行

2. **PropertiesJson 格式相容**
   - 保持現有屬性名稱 (Atk, Def, etc.)
   - 新增屬性使用相同格式

3. **前端顏色一致性**
   - 使用 TailwindCSS class
   - 品質顏色定義在常數中

4. **資料庫備份**
   - 遷移前備份 game.db
   - 測試完成後可還原
