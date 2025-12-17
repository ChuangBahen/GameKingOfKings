# Tasks: UI 優化

**Input**: Design documents from `/specs/001-ui-optimization/`
**Prerequisites**: spec.md (5 user stories), plan.md (技術實作計劃)

**Tests**: 不包含測試任務（未明確要求）

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Backend**: `Backend/` (ASP.NET Core 9.0 + EF Core + SQLite + SignalR)
- **Frontend**: `Frontend/src/` (Vue 3 + Vite + TailwindCSS + Pinia)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: 建立 DTO 與 SignalR 推送基礎架構

- [ ] T001 [P] Create GameDTOs.cs with PlayerFullStats, InventoryData, MapData, SkillsData in `Backend/DTOs/GameDTOs.cs`
- [ ] T002 [P] Create TypeScript interfaces for all DTOs in `Frontend/src/types/game.ts`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: 建立 SignalR 推送方法與前端狀態管理，所有 User Story 都依賴此階段

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T003 Add PushFullStats(), PushInventory(), PushMapData(), PushSkills() methods in `Backend/Hubs/GameHub.cs`
- [ ] T004 Modify JoinGame() to push all initial data on login in `Backend/Hubs/GameHub.cs`
- [ ] T005 Extend player store with fullStats, inventory, mapData, skills, messages state in `Frontend/src/stores/player.ts`
- [ ] T006 Add message classification logic (combat/general/system) in `Frontend/src/stores/player.ts`
- [ ] T007 Add onFullStatsUpdate(), onInventoryUpdate(), onMapUpdate(), onSkillsUpdate() event handlers in `Frontend/src/services/gameHub.ts`
- [ ] T008 Register all new SignalR event listeners in `Frontend/src/App.vue`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - 角色狀態即時同步 (Priority: P1) 🎯 MVP

**Goal**: 玩家能即時看到完整角色狀態（HP、MP、EXP、五維屬性、裝備加成）

**Independent Test**: 登入遊戲後查看狀態面板，進行戰鬥或使用物品後驗證數值是否即時更新

### Implementation for User Story 1

- [ ] T009 [US1] Add FullStatsUpdate trigger after combat damage in `Backend/Services/CombatManager.cs`
- [ ] T010 [US1] Add FullStatsUpdate trigger on level up in `Backend/Services/CombatManager.cs`
- [ ] T011 [US1] Add FullStatsUpdate trigger in HandleRest() in `Backend/Services/GameEngine.cs`
- [ ] T012 [US1] Add FullStatsUpdate trigger in HandleEquip()/HandleUnequip() in `Backend/Services/GameEngine.cs`
- [ ] T013 [US1] Add FullStatsUpdate trigger in HandleUseItem() in `Backend/Services/GameEngine.cs`
- [ ] T014 [US1] Update StatusPanel to display 5 primary stats (STR/DEX/INT/WIS/CON) in `Frontend/src/components/StatusPanel.vue`
- [ ] T015 [US1] Update StatusPanel to display equipment bonuses (ATK/DEF) in `Frontend/src/components/StatusPanel.vue`
- [ ] T016 [US1] Bind StatusPanel to fullStats reactive data in `Frontend/src/components/StatusPanel.vue`
- [ ] T017 [US1] Add disconnection status indicator in StatusPanel in `Frontend/src/components/StatusPanel.vue`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - 訊息視窗分離 (Priority: P1)

**Goal**: 戰鬥訊息與一般訊息分開顯示，各有獨立視窗

**Independent Test**: 進入戰鬥查看戰鬥視窗是否只顯示戰鬥相關訊息，一般視窗是否排除戰鬥訊息

### Implementation for User Story 2

- [ ] T018 [US2] Add message type field (combat/general/system) to ReceiveMessage in `Backend/Hubs/GameHub.cs`
- [ ] T019 [US2] Update combat messages to include type="combat" in `Backend/Services/CombatManager.cs`
- [ ] T020 [US2] Update movement/room messages to include type="general" in `Backend/Services/GameEngine.cs`
- [ ] T021 [US2] Filter GameTerminal to show only general messages in `Frontend/src/components/GameTerminal.vue`
- [ ] T022 [US2] Filter CombatView to show only combat messages in `Frontend/src/components/CombatView.vue`
- [ ] T023 [US2] Add message tab switching UI with unread count badges in `Frontend/src/App.vue`
- [ ] T024 [US2] Implement message count limit (100 per type) in `Frontend/src/stores/player.ts`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - 背包系統優化 (Priority: P2)

**Goal**: 背包以條列式顯示物品詳細資訊，區分已裝備與未裝備物品

**Independent Test**: 查看背包面板驗證物品資訊是否完整顯示，獲得新物品後背包是否即時更新

### Implementation for User Story 3

- [ ] T025 [US3] Add InventoryUpdate trigger after combat victory (loot) in `Backend/Services/CombatManager.cs`
- [ ] T026 [US3] Add InventoryUpdate trigger in HandleEquip()/HandleUnequip() in `Backend/Services/GameEngine.cs`
- [ ] T027 [US3] Add InventoryUpdate trigger in HandleUseItem() in `Backend/Services/GameEngine.cs`
- [ ] T028 [US3] Refactor InventoryPanel to list-style display in `Frontend/src/components/InventoryPanel.vue`
- [ ] T029 [US3] Add equipped/backpack item separation in `Frontend/src/components/InventoryPanel.vue`
- [ ] T030 [US3] Add item detail tooltip with properties in `Frontend/src/components/InventoryPanel.vue`
- [ ] T031 [US3] Bind InventoryPanel to inventory reactive data in `Frontend/src/components/InventoryPanel.vue`
- [ ] T032 [US3] Add gold display update in `Frontend/src/components/InventoryPanel.vue`

**Checkpoint**: At this point, User Stories 1, 2 AND 3 should all work independently

---

## Phase 6: User Story 4 - 小地圖即時更新 (Priority: P2)

**Goal**: 小地圖顯示當前房間名稱與可移動方向，移動時即時更新

**Independent Test**: 移動指令驗證小地圖是否正確更新當前位置和可用出口

### Implementation for User Story 4

- [ ] T033 [US4] Add MapUpdate trigger in HandleMove() in `Backend/Services/GameEngine.cs`
- [ ] T034 [US4] Add MapUpdate trigger on JoinGame() in `Backend/Hubs/GameHub.cs`
- [ ] T035 [US4] Refactor MiniMap to dynamic SVG rendering in `Frontend/src/components/MiniMap.vue`
- [ ] T036 [US4] Display current room name and available exits in `Frontend/src/components/MiniMap.vue`
- [ ] T037 [US4] Add monster presence indicator for adjacent rooms in `Frontend/src/components/MiniMap.vue`
- [ ] T038 [US4] Bind MiniMap to mapData reactive data in `Frontend/src/components/MiniMap.vue`

**Checkpoint**: At this point, User Stories 1-4 should all work independently

---

## Phase 7: User Story 5 - 職業技能系統 (Priority: P3)

**Goal**: 顯示職業技能列表，區分已學習與未解鎖技能

**Independent Test**: 查看技能面板驗證是否顯示正確的職業技能、學習狀態和使用說明

### Implementation for User Story 5

- [ ] T039 [US5] Add SkillsUpdate trigger on level up in `Backend/Services/CombatManager.cs`
- [ ] T040 [US5] Add SkillsUpdate trigger on JoinGame() in `Backend/Hubs/GameHub.cs`
- [ ] T041 [P] [US5] Create SkillPanel component in `Frontend/src/components/SkillPanel.vue`
- [ ] T042 [US5] Display learned skills list in SkillPanel in `Frontend/src/components/SkillPanel.vue`
- [ ] T043 [US5] Display locked skills with unlock requirements in `Frontend/src/components/SkillPanel.vue`
- [ ] T044 [US5] Add skill detail tooltip (MP cost, cooldown, description) in `Frontend/src/components/SkillPanel.vue`
- [ ] T045 [US5] Bind SkillPanel to skills reactive data in `Frontend/src/components/SkillPanel.vue`
- [ ] T046 [US5] Add SkillPanel to game layout in `Frontend/src/App.vue`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T047 Adjust UI layout to accommodate all panels in `Frontend/src/App.vue`
- [ ] T048 Add loading states for all panels when data not yet received in `Frontend/src/components/`
- [ ] T049 Verify all SignalR events are properly triggered and received
- [ ] T050 Performance optimization - debounce rapid state updates in `Frontend/src/stores/player.ts`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-7)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Phase 8)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 4 (P2)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 5 (P3)**: Can start after Foundational (Phase 2) - No dependencies on other stories

### Within Each User Story

- Backend triggers before frontend updates
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- T001, T002: Setup tasks can run in parallel
- T003-T008: Foundational tasks are sequential (GameHub → Store → Events)
- Once Foundational phase completes, all user stories can start in parallel
- Within US3: T025, T026, T027 can run in parallel (different trigger locations)
- Within US5: T039, T040 can run in parallel (different trigger locations)

---

## Parallel Example: User Story 3

```bash
# Launch all backend trigger tasks together:
Task: "Add InventoryUpdate trigger after combat victory in Backend/Services/CombatManager.cs"
Task: "Add InventoryUpdate trigger in HandleEquip()/HandleUnequip() in Backend/Services/GameEngine.cs"
Task: "Add InventoryUpdate trigger in HandleUseItem() in Backend/Services/GameEngine.cs"

# Then launch frontend updates:
Task: "Refactor InventoryPanel to list-style display in Frontend/src/components/InventoryPanel.vue"
```

---

## Implementation Strategy

### MVP First (User Story 1 + 2 Only)

1. Complete Phase 1: Setup (DTOs)
2. Complete Phase 2: Foundational (SignalR + Store)
3. Complete Phase 3: User Story 1 (角色狀態即時同步)
4. Complete Phase 4: User Story 2 (訊息視窗分離)
5. **STOP and VALIDATE**: Test both stories independently
6. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → 角色狀態即時同步 OK
3. Add User Story 2 → Test independently → 訊息視窗分離 OK (MVP!)
4. Add User Story 3 → Test independently → 背包系統優化 OK
5. Add User Story 4 → Test independently → 小地圖即時更新 OK
6. Add User Story 5 → Test independently → 職業技能系統 OK
7. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1 + 2 (P1 priority)
   - Developer B: User Story 3 + 4 (P2 priority)
   - Developer C: User Story 5 (P3 priority)
3. Stories complete and integrate independently

---

## Summary

| Metric | Value |
|--------|-------|
| Total Tasks | 50 |
| Setup Tasks | 2 |
| Foundational Tasks | 6 |
| US1 Tasks | 9 |
| US2 Tasks | 7 |
| US3 Tasks | 8 |
| US4 Tasks | 6 |
| US5 Tasks | 8 |
| Polish Tasks | 4 |
| Parallel Opportunities | 12 |
| MVP Scope | US1 + US2 (P1 priority) |

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
