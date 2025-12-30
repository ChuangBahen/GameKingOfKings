# Quickstart: 小地圖優化 (MiniMap Enhancement)

**Date**: 2025-12-30
**Feature**: 001-minimap-enhancement

## 快速開始

### 前置需求

- Node.js 18+
- pnpm 或 npm

### 啟動開發環境

```bash
# 進入前端目錄
cd Frontend

# 安裝依賴
npm install

# 啟動開發伺服器
npm run dev
```

### 需要的後端

確保後端已啟動（SignalR 連線需要）：

```bash
cd Backend
dotnet run
```

---

## 修改範圍

本功能只修改一個檔案：

```
Frontend/src/components/MiniMap.vue
```

### 不需要修改的檔案

- `Frontend/src/types/game.ts` - 介面已完整
- `Frontend/src/stores/player.ts` - Store 已有 mapData
- `Frontend/src/services/gameHub.ts` - 已有 sendCommand
- `Backend/*` - 無需後端修改

---

## 驗證步驟

### 1. 六方向顯示（P1 - Bug 修正）

1. 登入遊戲
2. 移動到有「上」或「下」出口的房間
3. 確認小地圖右上角（上）或左下角（下）顯示紫色節點

### 2. 危險警告（P2）

1. 找到相鄰有怪物的房間
2. 確認該方向出口顯示紅色

### 3. 點擊移動（P2）

1. 點擊小地圖上的出口節點
2. 確認角色移動到對應房間

### 4. Tooltip（P3）

1. 懸停在出口節點上
2. 確認顯示目標房間名稱
3. 如有怪物，確認顯示「有怪物出沒」

---

## 測試資料

如需測試上下方向，確保後端有設定帶有 up/down 出口的房間。

---

## 常見問題

### Q: 為什麼看不到上/下出口？

A: 確認後端 API 返回的 exits 包含 direction 為 "up" 或 "down" 的項目。

### Q: 點擊出口沒反應？

A: 檢查瀏覽器 Console 是否有 SignalR 連線錯誤。

### Q: 顏色不正確？

A: 確認 ExitInfo.hasMonsters 欄位有正確返回布林值。
