# Research: 小地圖優化 (MiniMap Enhancement)

**Date**: 2025-12-30
**Feature**: 001-minimap-enhancement

## 研究目標

分析現有 MiniMap.vue 實作，確定修改策略和最佳實踐。

---

## 1. 現有程式碼分析

### 1.1 上下方向未顯示的 Bug 根因

**Decision**: Bug 位於 v-for 遍歷邏輯，只遍歷 4 個方向

**Rationale**:
現有程式碼第 57-66 行和第 69-93 行的 v-for 使用：
```vue
v-for="dir in ['north', 'south', 'east', 'west']"
```
但 `directionPositions` 已定義 6 個方向（包含 up/down）。修正方式是將遍歷改為 6 個方向。

**Alternatives considered**:
- ❌ 重寫整個 SVG 邏輯：過度設計
- ✅ 修改 v-for 陣列：最小改動，符合 Constitution

### 1.2 hasMonsters 欄位未使用

**Decision**: 在 getExitColor 函數中檢查 hasMonsters

**Rationale**:
ExitInfo 介面已有 `hasMonsters: boolean` 欄位，但現有程式碼只檢查 `hasExit(dir)` 來決定顏色。需新增條件判斷。

**Implementation approach**:
```typescript
const getExitColor = (dir: string): string => {
  const exitInfo = getExitInfo(dir)
  if (!exitInfo) return '#374151' // 灰色 - 無出口
  if (exitInfo.hasMonsters) return '#ef4444' // 紅色 - 危險
  return '#4ade80' // 綠色 - 安全
}
```

### 1.3 點擊移動功能

**Decision**: 使用 gameHub.sendCommand 發送移動指令

**Rationale**:
現有 `gameHub.ts` 已有 `sendCommand(cmd: string)` 方法，移動指令格式為 `go [direction]`。

**Implementation approach**:
```typescript
import { gameHub } from '../services/gameHub'

const handleExitClick = (dir: string) => {
  if (hasExit(dir)) {
    gameHub.sendCommand(`go ${dir}`)
  }
}
```

**Alternatives considered**:
- ❌ 新增專用 API：過度設計，違反 Constitution
- ✅ 使用現有 sendCommand：符合現有架構

---

## 2. 顏色系統設計

### 2.1 顏色常數定義

**Decision**: 建立 COLORS 常數物件

**Rationale**: 避免魔術數字，符合 Constitution 規範

```typescript
const COLORS = {
  safe: '#4ade80',      // 綠色 - 安全出口
  danger: '#ef4444',    // 紅色 - 有怪物
  noExit: '#374151',    // 灰色 - 無出口
  player: '#3b82f6',    // 藍色 - 玩家位置
  vertical: '#a855f7'   // 紫色 - 上下方向
} as const
```

### 2.2 上下方向特殊顏色

**Decision**: 上下方向使用紫色區分，但危險時仍顯示紅色

**Rationale**: 紫色用於區分樓層概念，但安全性資訊更重要

**Priority order**:
1. 紅色（危險）> 紫色（上下）> 綠色（安全）

---

## 3. 方向位置座標確認

### 3.1 現有定義（維持不變）

```typescript
const directionPositions = {
  north: { x: 100, y: 40 },   // 正上方
  south: { x: 100, y: 160 },  // 正下方
  east:  { x: 160, y: 100 },  // 正右方
  west:  { x: 40, y: 100 },   // 正左方
  up:    { x: 140, y: 40 },   // 右上角
  down:  { x: 60, y: 160 }    // 左下角
} as const
```

**Decision**: 維持現有座標定義

**Rationale**: 座標已經過設計，右上角(up)和左下角(down)符合視覺直覺

---

## 4. Tooltip 實作方式

### 4.1 SVG title 元素

**Decision**: 使用現有 SVG `<title>` 元素擴展

**Rationale**:
現有程式碼已使用 `<title>` 提供 tooltip：
```vue
<title v-if="hasExit(dir)">{{ getExitInfo(dir)?.roomName || '未知' }}</title>
```

只需擴展內容包含怪物警告：
```vue
<title v-if="hasExit(dir)">
  {{ getExitInfo(dir)?.roomName || '未知' }}
  {{ getExitInfo(dir)?.hasMonsters ? ' - 有怪物出沒' : '' }}
</title>
```

**Alternatives considered**:
- ❌ 使用 Vue tooltip 套件：增加依賴，過度設計
- ✅ 使用原生 SVG title：簡單，瀏覽器原生支援

---

## 5. 效能考量

### 5.1 響應式計算

**Decision**: 維持現有 computed 模式

**Rationale**:
- mapData 從 PlayerStore 取得，已是響應式
- hasExit, getExitInfo 是輕量函數調用
- 無需額外優化

### 5.2 點擊事件

**Decision**: 直接發送指令，不做樂觀更新

**Rationale**:
- 後端會推送 MapUpdate 更新地圖
- 前端無需預測移動結果
- 簡化邏輯，避免狀態不一致

---

## 6. 技術結論

| 項目 | 決策 | 風險 |
|------|------|------|
| Bug 修正 | 修改 v-for 陣列 | 低 |
| 危險警告 | 新增 getExitColor 函數 | 低 |
| 點擊移動 | 使用 gameHub.sendCommand | 低 |
| Tooltip | 擴展 SVG title | 低 |
| 顏色系統 | COLORS 常數物件 | 低 |

**Overall Risk**: 低 - 這是純前端 UI 修改，不涉及後端或資料模型變更。
