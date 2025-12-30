# Feature Specification: 小地圖優化 (MiniMap Enhancement)

**Feature Branch**: `001-minimap-enhancement`
**Created**: 2025-12-30
**Status**: Draft
**Input**: 優化現有小地圖組件的 UI/UX，修正 Bug（上下方向未顯示），增加危險區域警告和快速移動互動功能

## User Scenarios & Testing *(mandatory)*

### User Story 1 - 六方向導航顯示 (Priority: P1)

玩家進入遊戲房間後，能在小地圖上看到所有 6 個可能的移動方向（北、南、東、西、上、下），清楚了解當前位置的出口選項。

**Why this priority**: 這是核心導航功能的 Bug 修正。目前只顯示 4 個方向，上下方向的出口無法在小地圖上看到，直接影響玩家探索體驗。

**Independent Test**: 進入有「上」或「下」出口的房間，確認小地圖正確顯示該方向的出口節點。

**Acceptance Scenarios**:

1. **Given** 玩家進入一個有北、南、上出口的房間, **When** 查看小地圖, **Then** 看到北方、南方顯示綠色連接線，上方（右上角）顯示紫色連接線，其餘方向顯示灰色虛線
2. **Given** 玩家進入一個有東、西、下出口的房間, **When** 查看小地圖, **Then** 看到東方、西方顯示綠色連接線，下方（左下角）顯示紫色連接線
3. **Given** 玩家在只有 4 個方向出口的房間, **When** 查看小地圖, **Then** 維持現有顯示方式，上下區域不顯示任何節點

---

### User Story 2 - 危險區域警告 (Priority: P2)

玩家在小地圖上能識別哪些相鄰房間有怪物出沒，通過紅色警告標記提前做出安全判斷。

**Why this priority**: 增強玩家的策略決策能力，讓玩家在移動前了解風險，提升遊戲體驗深度。

**Independent Test**: 進入有相鄰危險房間的位置，確認該方向出口顯示為紅色警告。

**Acceptance Scenarios**:

1. **Given** 玩家在一個房間，北邊相鄰房間有怪物, **When** 查看小地圖, **Then** 北方出口節點顯示紅色而非綠色
2. **Given** 玩家將滑鼠懸停在紅色出口節點上, **When** 停留片刻, **Then** 顯示提示訊息包含「有怪物出沒」警告
3. **Given** 玩家在一個房間，所有相鄰房間都沒有怪物, **When** 查看小地圖, **Then** 所有有效出口都顯示綠色

---

### User Story 3 - 快速移動互動 (Priority: P2)

玩家可以直接點擊小地圖上的出口節點來移動到相鄰房間，無需輸入文字指令。

**Why this priority**: 提升操作便利性，減少重複輸入移動指令的繁瑣，特別是在探索時能快速導航。

**Independent Test**: 點擊小地圖上的有效出口節點，確認角色移動到對應方向的房間。

**Acceptance Scenarios**:

1. **Given** 玩家看到北方有出口, **When** 點擊北方出口節點, **Then** 系統發送「go north」指令，玩家移動到北邊房間
2. **Given** 玩家看到上方有出口, **When** 點擊上方出口節點, **Then** 系統發送「go up」指令，玩家移動到上層
3. **Given** 玩家正在戰鬥中, **When** 點擊任何出口節點, **Then** 系統發送移動指令（後端處理戰鬥中無法移動的邏輯）
4. **Given** 某方向沒有出口（灰色虛線）, **When** 點擊該區域, **Then** 無任何反應

---

### User Story 4 - 出口資訊提示 (Priority: P3)

玩家懸停在出口節點上時，能看到目標房間的名稱，幫助決定移動方向。

**Why this priority**: 增強導航資訊，但不影響基本功能使用，屬於體驗優化。

**Independent Test**: 懸停在出口節點上，確認顯示目標房間名稱的 tooltip。

**Acceptance Scenarios**:

1. **Given** 玩家將滑鼠懸停在綠色北方出口節點, **When** 停留超過 0.5 秒, **Then** 顯示目標房間名稱（如「森林入口」）
2. **Given** 出口目標房間有怪物, **When** 懸停該節點, **Then** tooltip 同時顯示房間名稱和「有怪物出沒」警告

---

### Edge Cases

- **無出口房間**: 當房間沒有任何出口時，所有 6 個方向都顯示灰色虛線，底部顯示「無出口」文字
- **網路延遲**: 點擊移動後如果網路延遲，UI 維持原狀態等待後端回應更新
- **快速連點**: 連續快速點擊不同出口，每次點擊都發送對應指令（由後端處理衝突）
- **地圖資料未載入**: 地圖資料為空時，顯示預設的中心點和「載入中...」提示

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: 小地圖 MUST 顯示所有 6 個方向（北、南、東、西、上、下）的出口狀態
- **FR-002**: 有出口的方向 MUST 顯示連接線和可互動的圓形節點
- **FR-003**: 沒有出口的方向 MUST 顯示灰色虛線
- **FR-004**: 上/下方向的出口 MUST 使用紫色區分，位於右上角（上）和左下角（下）
- **FR-005**: 相鄰房間有怪物時，該出口 MUST 顯示紅色警告標記
- **FR-006**: 玩家 MUST 能夠點擊出口節點觸發對應方向的移動指令
- **FR-007**: 懸停出口節點時 MUST 顯示目標房間名稱的 tooltip
- **FR-008**: 現有功能（當前房間名稱、怪物數量警告、出口列表文字）MUST 維持正常運作
- **FR-009**: 中心玩家位置 MUST 維持藍色脈動動畫效果

### Key Entities

- **MapData**: 地圖資料，包含當前房間資訊和出口列表
- **RoomInfo**: 房間資訊，包含 id、name、description、monsters 列表
- **ExitInfo**: 出口資訊，包含 direction、roomId、roomName、hasMonsters 標記

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 所有 6 個方向的出口都能正確顯示，包含之前未顯示的上/下方向
- **SC-002**: 玩家能在 1 秒內識別哪些方向有危險（紅色標記）
- **SC-003**: 點擊出口後，移動指令在 100ms 內發送到伺服器
- **SC-004**: 懸停 tooltip 在 500ms 內顯示目標房間名稱
- **SC-005**: 現有的小地圖功能（房間名、怪物數、出口列表）100% 維持正常運作

## Assumptions

- 後端 API 已提供完整的 `ExitInfo.hasMonsters` 資料
- 後端 API 已支援上/下方向的出口資料
- 移動指令格式為 `go [direction]`（如 go north, go up）
- 玩家 store 中的 `mapData` 會即時更新
