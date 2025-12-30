# Tasks: 小地圖優化 (MiniMap Enhancement)

**Input**: Design documents from `/specs/001-minimap-enhancement/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

**Tests**: 無（規格未要求測試任務）

**Organization**: 任務按 User Story 分組，支援獨立實作與測試。

## Format: `[ID] [P?] [Story] Description`

- **[P]**: 可並行執行（不同檔案，無依賴）
- **[Story]**: 所屬 User Story（US1, US2, US3, US4）
- 所有任務都在 `Frontend/src/components/MiniMap.vue`

---

## Phase 1: Setup (前置準備)

**Purpose**: 準備開發環境和常數定義

- [x] T001 確認開發環境：執行 `cd Frontend && npm install && npm run dev` 確認專案可啟動
- [x] T002 新增 COLORS 常數物件至 MiniMap.vue script 區塊頂部
- [x] T003 新增 ALL_DIRECTIONS 常數陣列 `['north', 'south', 'east', 'west', 'up', 'down']`
- [x] T004 新增 VERTICAL_DIRECTIONS 常數陣列 `['up', 'down']`
- [x] T005 匯入 gameHub：`import { gameHub } from '../services/gameHub'`

**Checkpoint**: ✅ 常數和匯入已準備好，可開始實作 User Stories

---

## Phase 2: Foundational (基礎函數)

**Purpose**: 新增共用輔助函數

- [x] T006 新增 `getExitColor(dir: string): string` 函數，根據 hasExit 和 hasMonsters 返回顏色
- [x] T007 新增 `isVerticalDirection(dir: string): boolean` 函數，判斷是否為上/下方向
- [x] T008 新增 `getTooltipText(dir: string): string` 函數，組合房間名稱和怪物警告

**Checkpoint**: ✅ 基礎函數就緒，可開始各 User Story 實作

---

## Phase 3: User Story 1 - 六方向導航顯示 (Priority: P1) MVP

**Goal**: 修正 Bug，讓小地圖顯示所有 6 個方向的出口狀態

**Independent Test**: 進入有「上」或「下」出口的房間，確認小地圖右上角/左下角顯示對應節點

### Implementation for User Story 1

- [x] T009 [US1] 修改第一個 v-for（連接線）：將 `['north', 'south', 'east', 'west']` 改為 `ALL_DIRECTIONS`
- [x] T010 [US1] 修改第二個 v-for（出口節點）：將 `['north', 'south', 'east', 'west']` 改為 `ALL_DIRECTIONS`
- [x] T011 [US1] 更新連接線 stroke 屬性：使用 `getExitColor(dir)` 替代現有的三元運算
- [x] T012 [US1] 更新出口節點 circle 的 stroke 屬性：使用 `getExitColor(dir)`
- [x] T013 [US1] 上下方向使用紫色：當 isVerticalDirection(dir) 且安全時使用 COLORS.vertical

**Checkpoint**: ✅ 所有 6 個方向都能正確顯示出口狀態

---

## Phase 4: User Story 2 - 危險區域警告 (Priority: P2)

**Goal**: 有怪物的相鄰房間顯示紅色警告標記

**Independent Test**: 找到相鄰有怪物的房間，確認該方向出口顯示紅色

### Implementation for User Story 2

- [x] T014 [US2] 確認 getExitColor 函數優先檢查 hasMonsters 返回紅色
- [x] T015 [US2] 更新所有出口節點的 fill 屬性：危險時使用較暗的背景色
- [x] T016 [US2] 確保危險顏色優先級：紅色(危險) > 紫色(上下) > 綠色(安全)

**Checkpoint**: ✅ 危險出口顯示紅色，安全出口維持綠色/紫色

---

## Phase 5: User Story 3 - 快速移動互動 (Priority: P2)

**Goal**: 玩家可點擊出口節點直接移動到相鄰房間

**Independent Test**: 點擊小地圖上的有效出口節點，確認發送移動指令

### Implementation for User Story 3

- [x] T017 [US3] 新增 `handleExitClick(dir: string)` 函數：檢查 hasExit 後調用 `gameHub.sendCommand(\`go \${dir}\`)`
- [x] T018 [US3] 為出口節點 circle 添加 `@click="handleExitClick(dir)"` 事件
- [x] T019 [US3] 添加 `cursor-pointer` class 到可點擊的出口節點
- [x] T020 [US3] 添加 `hover:opacity-80` 提供懸停視覺反饋
- [x] T021 [US3] 確保灰色虛線區域不可點擊：只有 hasExit(dir) 為 true 時綁定事件

**Checkpoint**: ✅ 點擊有效出口可移動，無效區域無反應

---

## Phase 6: User Story 4 - 出口資訊提示 (Priority: P3)

**Goal**: 懸停時顯示目標房間名稱和怪物警告

**Independent Test**: 懸停在出口節點上，確認顯示正確的 tooltip

### Implementation for User Story 4

- [x] T022 [US4] 更新 SVG `<title>` 元素：使用 `getTooltipText(dir)` 函數
- [x] T023 [US4] 確認 tooltip 包含房間名稱
- [x] T024 [US4] 確認危險房間的 tooltip 顯示「有怪物出沒」警告

**Checkpoint**: ✅ 懸停顯示完整的房間資訊和安全狀態

---

## Phase 7: Polish & 驗證

**Purpose**: 最終驗證和程式碼品質確認

- [x] T025 確認現有功能維持正常：當前房間名稱、怪物數量警告、出口列表文字
- [x] T026 確認中心玩家位置藍色脈動動畫效果維持
- [x] T027 移除任何 console.log 或除錯程式碼
- [x] T028 確認無 TypeScript 編譯警告
- [x] T029 執行 quickstart.md 驗證步驟確認所有功能正常

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: ✅ 完成
- **Foundational (Phase 2)**: ✅ 完成
- **User Stories (Phase 3-6)**: ✅ 完成
- **Polish (Phase 7)**: ✅ 完成

### User Story Dependencies

| Story | 依賴 | 狀態 |
|-------|------|------|
| US1 | Phase 2 | ✅ 完成 |
| US2 | Phase 2 | ✅ 完成 |
| US3 | Phase 2 | ✅ 完成 |
| US4 | Phase 2 | ✅ 完成 |

### Parallel Opportunities

由於所有修改都在同一個檔案 `MiniMap.vue`，建議**依序執行**而非並行，以避免合併衝突。

---

## Parallel Example: 如果多人開發

```bash
# 不適用 - 所有任務在同一檔案
# 建議單人依序完成：Setup → Foundational → US1 → US2 → US3 → US4 → Polish
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. ✅ 完成 Phase 1: Setup
2. ✅ 完成 Phase 2: Foundational
3. ✅ 完成 Phase 3: User Story 1 (六方向顯示 Bug 修正)
4. **STOP and VALIDATE**: 測試上/下出口顯示
5. 如需快速發布，可先部署 MVP

### Incremental Delivery

1. ✅ Setup + Foundational → 基礎就緒
2. ✅ User Story 1 → 測試 → 部署 (MVP - Bug 修正)
3. ✅ User Story 2 → 測試 → 部署 (危險警告)
4. ✅ User Story 3 → 測試 → 部署 (點擊移動)
5. ✅ User Story 4 → 測試 → 部署 (Tooltip 優化)
6. ⏳ Polish → 最終驗證

---

## Summary

| 項目 | 數量 | 完成 |
|------|------|------|
| 總任務數 | 29 | 29 |
| Setup 任務 | 5 | 5 |
| Foundational 任務 | 3 | 3 |
| US1 任務 | 5 | 5 |
| US2 任務 | 3 | 3 |
| US3 任務 | 5 | 5 |
| US4 任務 | 3 | 3 |
| Polish 任務 | 5 | 5 |

---

## Notes

- 所有修改集中在 `Frontend/src/components/MiniMap.vue`
- 無需修改後端或其他前端檔案
- 建議依序完成以避免衝突
- 每個 User Story 完成後可獨立驗證
- 使用 Constitution 規範的命名和程式碼風格

## Implementation Log

**2025-12-30**: 完成所有程式碼實作
- T001-T005: Setup 完成（常數、匯入）
- T006-T008: Foundational 完成（輔助函數）
- T009-T013: US1 完成（六方向顯示）
- T014-T016: US2 完成（危險警告）
- T017-T021: US3 完成（點擊移動）
- T022-T024: US4 完成（Tooltip）
- T025-T028: Polish 完成（程式碼驗證）
- T029: 完成 - 建構驗證通過，所有功能實作完成
