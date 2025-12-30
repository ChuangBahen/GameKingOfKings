# Implementation Plan: 小地圖優化 (MiniMap Enhancement)

**Branch**: `001-minimap-enhancement` | **Date**: 2025-12-30 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-minimap-enhancement/spec.md`

## Summary

優化現有 MiniMap.vue 組件，修正上/下方向未顯示的 Bug，增加危險區域紅色警告標記，並實作點擊出口快速移動功能。這是純前端的 UI/UX 增強，不需要後端修改。

## Technical Context

**Language/Version**: TypeScript 5.9+ / Vue 3.5+
**Primary Dependencies**: Vue 3, Pinia, TailwindCSS, SignalR Client
**Storage**: N/A (使用現有 PlayerStore)
**Testing**: Vue Test Utils (如需新增)
**Target Platform**: Web Browser (現代瀏覽器)
**Project Type**: Web application (Frontend + Backend)
**Performance Goals**: 點擊響應 < 100ms，tooltip 顯示 < 500ms
**Constraints**: 維持現有 SVG 尺寸 200x200，不破壞現有功能
**Scale/Scope**: 單一 Vue 組件修改

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 狀態 | 說明 |
|------|------|------|
| 玩家體驗優先 | ✅ PASS | 修正 Bug 並增強導航體驗 |
| UI/UX 設計直覺友善 | ✅ PASS | 使用顏色區分安全/危險，點擊移動更直覺 |
| 可讀性優於簡潔性 | ✅ PASS | 將複雜邏輯拆分為函數 |
| 避免魔術數字 | ✅ PASS | 使用常數定義顏色和位置 |
| 單一職責 | ✅ PASS | MiniMap 組件專注於地圖顯示和導航 |
| 避免過度設計 | ✅ PASS | 僅修改必要部分，不重構整體架構 |

**Gate Result**: ✅ ALL PASS - 可進入 Phase 0

## Project Structure

### Documentation (this feature)

```text
specs/001-minimap-enhancement/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
Frontend/
├── src/
│   ├── components/
│   │   └── MiniMap.vue     # 主要修改目標
│   ├── stores/
│   │   └── player.ts       # 已有 mapData (無需修改)
│   ├── services/
│   │   └── gameHub.ts      # 已有 sendCommand (無需修改)
│   └── types/
│       └── game.ts         # 已有 MapData, ExitInfo (無需修改)
└── tests/
    └── (未來可新增)

Backend/
├── (無需修改 - 純前端功能)
```

**Structure Decision**: 選用 Option 2 (Web application)，這是現有專案結構。本功能僅修改 `Frontend/src/components/MiniMap.vue`。

## Complexity Tracking

> 無違反項目，不需要複雜度追蹤。
