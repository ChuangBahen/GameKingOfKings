---
description: 使用 SDD (Spec-Driven Development) 流程開發新功能，整合多個專家 Agent 協作
---

# SDD 遊戲功能開發流程

## 輸入參數
功能描述: $ARGUMENTS

## 流程概覽

```
┌──────────────────────────────────────────────────────────────────┐
│                    SDD 開發流程 (SpecKit 整合版)                   │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Step 0: Constitution Check                                      │
│     └─→ /speckit.constitution (若不存在)                         │
│                                                                  │
│  Step 1: Design (game-designer Agent)                            │
│     └─→ 遊戲設計：機制、數值、體驗                                 │
│     └─→ 輸出：.claude/shared/designs/[feature]-design.md         │
│                                                                  │
│  Step 2: Specify                                                 │
│     └─→ /speckit.specify [功能描述]                              │
│     └─→ 輸出：.specify/specs/[N]-[feature]/spec.md               │
│                                                                  │
│  Step 3: Clarify (若有需要)                                       │
│     └─→ /speckit.clarify                                         │
│     └─→ 釐清需求中的模糊點                                        │
│                                                                  │
│  Step 4: Plan                                                    │
│     └─→ /speckit.plan                                            │
│     └─→ 輸出：plan.md, research.md, data-model.md                │
│                                                                  │
│  Step 5: Tasks                                                   │
│     └─→ /speckit.tasks                                           │
│     └─→ 輸出：tasks.md (依賴排序的任務清單)                       │
│                                                                  │
│  Step 6: Implement                                               │
│     └─→ /speckit.implement                                       │
│     └─→ 逐一執行任務、撰寫測試                                    │
│                                                                  │
│  Step 7: Analyze                                                 │
│     └─→ /speckit.analyze                                         │
│     └─→ 一致性檢查、最終驗收                                      │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

## Agent 與 SpecKit 整合表

| 步驟 | SpecKit 指令 | 輔助 Agent | 產出 |
|------|-------------|-----------|------|
| Constitution | `/speckit.constitution` | - | `constitution.md` |
| Design | (手動) | game-designer | `*-design.md` |
| Specify | `/speckit.specify` | spec-writer | `spec.md` |
| Clarify | `/speckit.clarify` | - | 更新 `spec.md` |
| Plan | `/speckit.plan` | tech-planner, architect | `plan.md`, `research.md` |
| Tasks | `/speckit.tasks` | task-breakdown | `tasks.md` |
| Implement | `/speckit.implement` | implementer, qa-tester | 程式碼 + 測試 |
| Analyze | `/speckit.analyze` | quality-reviewer | 一致性報告 |

## 執行流程

當你執行 `/sdd-develop [功能描述]` 時，我會按以下步驟執行：

---

### Step 0: Constitution 檢查

檢查 `.specify/memory/constitution.md` 是否存在。

**如果不存在**：
```
執行 /speckit.constitution 建立專案原則
```

**如果存在**：繼續下一步

---

### Step 1: 遊戲設計 (Design)

使用 **game-designer** Agent 進行遊戲設計：

```
啟動 game-designer Agent，設計功能：
- 分析功能在遊戲中的定位
- 設計核心玩法循環
- 規劃數值系統
- 設計玩家體驗
- 考慮與其他系統的互動
```

**輸出**：`.claude/shared/designs/[feature]-design.md`

**完成後**：展示設計文件，等待確認後進入下一步

---

### Step 2: 撰寫規格 (Specify)

執行 SpecKit 指令建立規格：

```
執行 /speckit.specify [功能描述]
```

這會：
1. 建立功能分支
2. 根據設計撰寫 spec.md
3. 產生規格品質檢查清單

**輸出**：`.specify/specs/[N]-[feature]/spec.md`

---

### Step 3: 釐清需求 (Clarify) [若有需要]

如果 spec.md 中有 `[NEEDS CLARIFICATION]` 標記：

```
執行 /speckit.clarify 釐清模糊需求
```

這會列出需要釐清的問題，等待你回答後更新規格。

---

### Step 4: 技術規劃 (Plan)

執行 SpecKit 指令建立技術計畫：

```
執行 /speckit.plan
```

這會：
1. 進行技術研究
2. 設計資料模型
3. 定義 API 契約
4. 建立快速啟動指南

**輸出**：`plan.md`, `research.md`, `data-model.md`, `contracts/`

---

### Step 5: 任務分解 (Tasks)

執行 SpecKit 指令分解任務：

```
執行 /speckit.tasks
```

這會：
1. 從 spec.md 和 plan.md 提取任務
2. 建立依賴關係
3. 識別可並行任務
4. 產生執行順序

**輸出**：`tasks.md` (依賴排序的任務清單)

---

### Step 6: 實作 (Implement)

執行 SpecKit 指令開始實作：

```
執行 /speckit.implement
```

這會：
1. 按順序執行 tasks.md 中的任務
2. 每個任務完成後執行測試
3. 更新任務狀態

---

### Step 7: 一致性分析 (Analyze)

執行 SpecKit 指令進行最終檢查：

```
執行 /speckit.analyze
```

這會：
1. 檢查 spec.md、plan.md、tasks.md 的一致性
2. 驗證所有驗收標準
3. 產出分析報告

---

## 流程控制

### 暫停與繼續
你可以在任何步驟暫停：
```
先暫停，我想調整一下設計
```

### 跳過步驟
對於簡單功能，可以跳過設計步驟直接從 specify 開始：
```
跳過 game-designer，直接 /speckit.specify
```

### 召喚特定 Agent
在任何步驟都可以召喚 Agent 諮詢：
```
請 architect 看一下這個架構
請 game-designer 確認這個數值平衡
```

### 手動執行單一步驟
你也可以直接執行單一 SpecKit 指令：
- `/speckit.specify [功能描述]` - 只做規格
- `/speckit.plan` - 只做計畫
- `/speckit.tasks` - 只做任務分解
- `/speckit.implement` - 只做實作

---

## 開始執行

我會自動執行上述流程。每個步驟完成後，我會：
1. 展示該步驟的產出
2. 詢問是否需要調整
3. 確認後執行下一步的 SpecKit 指令

**功能描述**: $ARGUMENTS

---

### 開始 Step 0: Constitution 檢查

讓我先檢查 `.specify/memory/constitution.md` 是否存在...
