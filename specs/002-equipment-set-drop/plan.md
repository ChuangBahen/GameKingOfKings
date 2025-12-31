# Implementation Plan: 套裝系統與裝備掉落機制

**Branch**: `002-equipment-set-drop` | **Date**: 2025-12-30 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-equipment-set-drop/spec.md`

## Summary

為遊戲裝備系統新增「套裝」和「品質」概念，讓玩家收集套裝部件獲得額外加成。同時改進怪物掉落機制，普通怪有極低機率(0.5-2%)掉落裝備，Boss 有高機率(30-100%)掉落。此功能涉及後端模型擴展、掉落邏輯修改、前端 UI 顏色和套裝資訊顯示。

## Technical Context

**Language/Version**: C# 12+ / TypeScript 5.9+ / Vue 3.5+
**Primary Dependencies**: ASP.NET Core, Entity Framework Core, SignalR, Pinia, TailwindCSS
**Storage**: SQLite (現有 game.db)
**Testing**: xUnit (後端), Vue Test Utils (前端)
**Target Platform**: Web Browser (現代瀏覽器) + .NET Backend
**Project Type**: Web application (Frontend + Backend)
**Performance Goals**: 掉落計算 < 50ms, 套裝加成計算 < 100ms, UI 更新即時
**Constraints**: 維持現有掉落系統(材料、藥水)不變, 資料庫遷移需向後相容
**Scale/Scope**: 3 個初始套裝, 4 個品質等級, 單一 Vue 組件修改

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| 原則 | 狀態 | 說明 |
|------|------|------|
| 玩家體驗優先 | ✅ PASS | 套裝收集增加遊戲深度，普通怪掉裝備增加驚喜感 |
| UI/UX 設計直覺友善 | ✅ PASS | 使用顏色區分品質(白/綠/藍/紫)，套裝資訊清楚顯示 |
| 可讀性優於簡潔性 | ✅ PASS | 使用 enum 定義品質，專用模型定義套裝 |
| 避免魔術數字 | ✅ PASS | 掉落率和品質機率使用常數定義 |
| 單一職責 | ✅ PASS | SetBonusService 專責套裝計算，DropService 專責掉落 |
| 依賴注入 | ✅ PASS | 所有服務透過 DI 注入 |
| 介面優先 | ✅ PASS | 新增 ISetBonusService, IEquipmentDropService |
| 錯誤處理 | ✅ PASS | 使用日誌記錄掉落事件，邊界情況有明確處理 |
| 避免過度設計 | ✅ PASS | 利用現有 PropertiesJson 模式，不重構整體架構 |

**Gate Result**: ✅ ALL PASS - 可進入 Phase 0

## Project Structure

### Documentation (this feature)

```text
specs/002-equipment-set-drop/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks)
```

### Source Code (repository root)

```text
Backend/
├── Models/
│   ├── Item.cs              # 新增 Quality, SetId 欄位
│   ├── EquipmentSet.cs      # 新增：套裝定義
│   └── SetBonus.cs          # 新增：套裝加成定義
├── Services/
│   ├── CombatManager.cs     # 修改：裝備掉落邏輯
│   ├── SetBonusService.cs   # 新增：套裝加成計算
│   └── GameEngine.cs        # 修改：整合套裝加成
├── DTOs/
│   └── GameDTOs.cs          # 新增：EquipmentSetDto, 修改 InventoryItemDto
├── Data/
│   └── AppDbContext.cs      # 新增 DbSet, 種子資料
└── Migrations/
    └── *_AddEquipmentSetSystem.cs  # 新增遷移

Frontend/
├── src/
│   ├── components/
│   │   └── InventoryPanel.vue  # 修改：品質顏色、套裝資訊顯示
│   ├── stores/
│   │   └── player.ts           # 修改：套裝加成狀態
│   └── types/
│       └── game.ts             # 新增：ItemQuality, EquipmentSetDto
└── (無需新增測試目錄)
```

**Structure Decision**: 選用 Option 2 (Web application)，這是現有專案結構。本功能主要修改 Backend/Models、Backend/Services 和 Frontend/src/components。

## Complexity Tracking

> 無違反項目，不需要複雜度追蹤。本功能遵循現有架構模式，使用 PropertiesJson 擴展方式。

## Technical Decisions

### TD-001: 品質系統實作方式

**Decision**: 在 Item 模型新增 Quality 欄位 (enum)

**Rationale**:
- 簡單直接，不需要額外 join
- 品質是裝備的固有屬性
- 方便在 UI 和計算中使用

**Alternatives Rejected**:
- PropertiesJson 中存品質：不利於查詢和統計
- 獨立 ItemQuality 表：過度設計

### TD-002: 套裝系統實作方式

**Decision**: 新增 EquipmentSet 和 SetBonus 模型，Item 新增 SetId nullable FK

**Rationale**:
- 套裝和加成是獨立概念，需要專用模型
- SetBonus 支援多階段加成 (2件/3件/4件)
- Item.SetId 允許 null (非套裝裝備)

**Alternatives Rejected**:
- 在 Item.PropertiesJson 存套裝資訊：不利於查詢套裝完成度
- 單一 SetBonus JSON：不夠結構化

### TD-003: 裝備掉落判定方式

**Decision**: 在 CombatManager 現有掉落邏輯後新增裝備掉落判定

**Rationale**:
- 不影響現有材料/藥水掉落
- 使用怪物 Level 和 IsBoss 決定機率
- 品質隨機使用權重機率

**Alternatives Rejected**:
- 修改 LootTableEntry：會影響現有資料
- 獨立 EquipmentDrop 表：過度設計

### TD-004: 套裝加成計算時機

**Decision**: 在 GetEquipmentBonusesAsync 中整合套裝加成計算

**Rationale**:
- 統一的裝備加成入口
- 避免多處重複計算
- 前端已經使用此資料

**Alternatives Rejected**:
- 獨立 API 端點：增加前端複雜度
- 每次裝備變更時預計算：需要額外儲存

## Dependencies

| 依賴 | 類型 | 說明 |
|------|------|------|
| Entity Framework Core | 現有 | 資料庫操作 |
| SignalR | 現有 | 即時推送掉落結果 |
| System.Text.Json | 現有 | PropertiesJson 序列化 |
| Pinia | 現有 | 前端狀態管理 |

## Risk Assessment

| 風險 | 機率 | 影響 | 緩解方式 |
|------|------|------|---------|
| 資料庫遷移失敗 | 低 | 高 | 備份 game.db，使用可逆遷移 |
| 現有掉落系統受影響 | 低 | 高 | 裝備掉落在現有邏輯之後執行 |
| 套裝加成計算效能 | 低 | 中 | 快取已裝備套裝資訊 |
| 前端顯示問題 | 中 | 低 | 漸進式修改，每階段驗證 |
