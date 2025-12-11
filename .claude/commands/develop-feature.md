---
description: 啟動新功能的多 Agent 協作開發流程
---

# 啟動功能開發流程

## 輸入參數
功能描述: $ARGUMENTS

## 執行步驟

### Step 1: 遊戲設計
使用 **game-designer** agent 進行遊戲設計：
1. 閱讀 `.claude/shared/game-design-doc.md` 了解遊戲背景
2. 分析功能需求
3. 設計遊戲機制與數值
4. 輸出設計文件到 `.claude/shared/designs/[feature-name].md`
5. 更新 `.claude/shared/context.md`

### Step 2: 架構設計
使用 **architect** agent 進行架構設計：
1. 閱讀遊戲設計文件
2. 分析現有程式碼結構
3. 設計系統架構與介面
4. 輸出架構文件到 `.claude/shared/architecture/[system-name].md`
5. 記錄重要決策到 `.claude/shared/decisions.md`

### Step 3: 程式實作
使用 **business-logic-dev** agent 進行開發：
1. 閱讀設計文件與架構文件
2. 實作程式碼
3. 確保編譯通過
4. 更新 `.claude/shared/context.md`

### Step 4: 測試驗證
使用 **qa-tester** agent 進行測試：
1. 閱讀設計文件了解預期行為
2. 撰寫測試案例
3. 執行測試
4. 產出測試報告到 `.claude/shared/test-reports/`
5. 記錄發現的問題到 `.claude/shared/review-findings.md`

### Step 5: 程式碼審查（可選）
如有需要，使用 **refactoring-expert** agent：
1. 審查程式碼品質
2. 提出重構建議
3. 執行必要的重構

## 開始執行
請先確認功能需求，然後從 Step 1 開始。每個步驟完成後，告訴我進度，我會指示下一步。
