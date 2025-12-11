# 遊戲專案 Multi-Agent 開發指南

本指南說明如何使用 Claude Code 的 Subagent 功能進行遊戲開發。

## 目錄結構

```
your-game-project/
├── .claude/
│   ├── agents/                    # Agent 定義檔
│   │   ├── game-designer.md       # 遊戲設計師
│   │   ├── architect.md           # 系統架構師
│   │   ├── refactoring-expert.md  # 重構專家
│   │   ├── business-logic-dev.md  # 商業邏輯開發專家
│   │   └── qa-tester.md           # 品質測試專家
│   │
│   ├── shared/                    # Agent 間共享資料
│   │   ├── context.md             # 專案上下文與狀態
│   │   ├── decisions.md           # 設計決策記錄
│   │   ├── review-findings.md     # 審查發現記錄
│   │   ├── game-design-doc.md     # 遊戲設計文件
│   │   ├── designs/               # 功能設計文件
│   │   ├── architecture/          # 架構設計文件
│   │   └── test-reports/          # 測試報告
│   │
│   └── commands/                  # 自訂指令
│       └── develop-feature.md     # 功能開發流程
│
├── src/                           # 原始碼
└── tests/                         # 測試程式碼
```

## 快速開始

### 1. 複製 Agent 設定到你的專案

將 `.claude` 資料夾複製到你的遊戲專案根目錄：

```bash
cp -r .claude /path/to/your-game-project/
```

### 2. 初始化共享文件

編輯以下檔案，填入你的專案資訊：

- `.claude/shared/context.md` - 填入專案名稱和當前狀態
- `.claude/shared/game-design-doc.md` - 填入遊戲設計資訊

### 3. 開始使用

在專案目錄啟動 Claude Code：

```bash
cd /path/to/your-game-project
claude
```

## 使用方式

### 方式一：使用自訂指令（推薦）

```
/develop-feature 實作背包系統，包含道具新增、移除、堆疊功能
```

這會自動啟動完整的開發流程。

### 方式二：直接呼叫特定 Agent

```
請使用 game-designer 來設計戰鬥系統的數值平衡
```

```
請使用 architect 分析現有程式碼並設計新的狀態管理架構
```

```
請使用 business-logic-dev 實作 InventoryService
```

```
請使用 qa-tester 為 InventoryService 撰寫單元測試
```

```
請使用 refactoring-expert 檢查並重構 PlayerController
```

### 方式三：讓 Claude 自動選擇

根據 Agent 的 description 設定，Claude 會自動判斷何時該使用哪個 Agent：

```
我想設計一個新的技能系統
→ 自動啟用 game-designer

這個類別太大了，需要拆分
→ 自動啟用 refactoring-expert

幫我寫 CombatService 的測試
→ 自動啟用 qa-tester
```

## Agent 角色說明

| Agent | 職責 | 何時使用 |
|-------|------|---------|
| **game-designer** | 遊戲機制設計、數值平衡、玩家體驗 | 設計新功能、調整遊戲平衡 |
| **architect** | 系統架構設計、技術選型、介面定義 | 新系統設計、架構重構 |
| **business-logic-dev** | 程式碼實作、商業邏輯開發 | 實作功能、寫程式碼 |
| **qa-tester** | 撰寫測試、驗證功能、找 Bug | 測試程式碼、品質保證 |
| **refactoring-expert** | 程式碼重構、消除技術債 | 改善程式碼品質 |

## Agent 協作流程

```
          ┌─────────────────┐
          │  game-designer  │  ← 遊戲設計
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │    architect    │  ← 架構設計
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │business-logic-dev│ ← 程式實作
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │    qa-tester    │  ← 測試驗證
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │refactoring-expert│ ← 重構優化（可選）
          └─────────────────┘
```

## 共享資訊機制

Agents 透過共享檔案進行間接溝通：

1. **context.md** - 專案狀態、進度追蹤、Agent 間訊息
2. **decisions.md** - 重要設計決策記錄
3. **review-findings.md** - 審查和測試發現的問題
4. **designs/** - 功能設計文件
5. **architecture/** - 架構設計文件
6. **test-reports/** - 測試報告

每個 Agent 在開始工作前會讀取相關共享檔案，完成後會更新狀態。

## 自訂與擴展

### 新增 Agent

在 `.claude/agents/` 建立新的 `.md` 檔案：

```markdown
---
name: your-agent-name
description: 描述何時應該觸發這個 Agent
model: sonnet
tools:
  - read_file
  - write_file
---

# 角色說明
...
```

### 修改現有 Agent

直接編輯對應的 `.md` 檔案，調整 prompt 或工具權限。

### 新增自訂指令

在 `.claude/commands/` 建立新的 `.md` 檔案。

## 最佳實踐

1. **保持共享文件更新** - 讓 Agents 有最新的上下文資訊
2. **先設計後實作** - 依照 設計 → 架構 → 實作 → 測試 的流程
3. **小步快跑** - 每次只處理一個功能或模組
4. **持續測試** - 實作後立即執行測試
5. **記錄決策** - 重要決策都記錄到 decisions.md

## 常見問題

### Q: Agent 沒有自動觸發？
A: 確認 Agent 的 description 寫得夠精確，或直接指定使用哪個 Agent。

### Q: Agent 之間如何溝通？
A: 透過 `.claude/shared/` 中的共享檔案間接溝通。

### Q: 可以同時執行多個 Agent 嗎？
A: 可以，Claude Code 支援同時執行最多 10 個 tasks。

### Q: 如何處理 Agent 間的衝突？
A: 透過主 Claude session 協調，或在 context.md 中留言討論。
