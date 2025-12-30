# Data Model: 小地圖優化 (MiniMap Enhancement)

**Date**: 2025-12-30
**Feature**: 001-minimap-enhancement

## 概述

本功能不需要新增資料模型，僅使用現有的 TypeScript 介面。以下記錄現有資料結構供參考。

---

## 現有實體（無需修改）

### MapData

**位置**: `Frontend/src/types/game.ts`

```typescript
export interface MapData {
  currentRoom: RoomInfo;
  exits: ExitInfo[];
}
```

| 屬性 | 類型 | 說明 |
|------|------|------|
| currentRoom | RoomInfo | 當前房間資訊 |
| exits | ExitInfo[] | 出口列表 |

---

### RoomInfo

**位置**: `Frontend/src/types/game.ts`

```typescript
export interface RoomInfo {
  id: number;
  name: string;
  description: string;
  monsters: string[];
}
```

| 屬性 | 類型 | 說明 |
|------|------|------|
| id | number | 房間唯一識別碼 |
| name | string | 房間名稱（顯示用） |
| description | string | 房間描述 |
| monsters | string[] | 房間內怪物名稱列表 |

---

### ExitInfo

**位置**: `Frontend/src/types/game.ts`

```typescript
export interface ExitInfo {
  direction: string;
  roomId: number;
  roomName: string;
  hasMonsters: boolean;  // ← 本功能將使用此欄位
}
```

| 屬性 | 類型 | 說明 |
|------|------|------|
| direction | string | 方向 (north/south/east/west/up/down) |
| roomId | number | 目標房間 ID |
| roomName | string | 目標房間名稱 |
| hasMonsters | boolean | 目標房間是否有怪物 **(本功能使用)** |

---

## 新增常數（MiniMap.vue 內部）

### COLORS

```typescript
const COLORS = {
  safe: '#4ade80',      // 綠色 - 安全出口
  danger: '#ef4444',    // 紅色 - 有怪物
  noExit: '#374151',    // 灰色 - 無出口
  player: '#3b82f6',    // 藍色 - 玩家位置
  vertical: '#a855f7'   // 紫色 - 上下方向（安全時）
} as const
```

### ALL_DIRECTIONS

```typescript
const ALL_DIRECTIONS = ['north', 'south', 'east', 'west', 'up', 'down'] as const
```

### VERTICAL_DIRECTIONS

```typescript
const VERTICAL_DIRECTIONS = ['up', 'down'] as const
```

---

## 狀態流程

```
┌─────────────────────────────────────────────────────────────┐
│                      資料流                                  │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Backend (SignalR)                                           │
│       │                                                      │
│       │ onMapUpdate(data: MapData)                          │
│       ▼                                                      │
│  PlayerStore.setMapData(data)                               │
│       │                                                      │
│       │ mapData (reactive)                                  │
│       ▼                                                      │
│  MiniMap.vue (computed)                                     │
│       │                                                      │
│       ├── currentRoom → 顯示房間名稱                        │
│       ├── exits → 渲染出口節點                              │
│       │    ├── hasExit(dir) → 是否有出口                   │
│       │    ├── getExitInfo(dir) → 取得出口資訊             │
│       │    └── getExitColor(dir) → 決定節點顏色            │
│       │                                                      │
│       └── handleExitClick(dir) → gameHub.sendCommand()      │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 變更摘要

| 類型 | 項目 | 變更 |
|------|------|------|
| 介面 | MapData | 無變更 |
| 介面 | RoomInfo | 無變更 |
| 介面 | ExitInfo | 無變更（開始使用 hasMonsters） |
| 常數 | COLORS | 新增（MiniMap.vue 內部） |
| 常數 | ALL_DIRECTIONS | 新增（MiniMap.vue 內部） |
