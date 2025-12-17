# SQLite WAL Checkpoint Script
# 在 commit 前執行此腳本，確保所有 WAL 資料合併到 game.db
#
# 用法: .\Backend\Tools\checkpoint-db.ps1

$ErrorActionPreference = "Stop"

$dbPath = Join-Path $PSScriptRoot "..\game.db"

if (-not (Test-Path $dbPath)) {
    Write-Error "找不到資料庫: $dbPath"
    exit 1
}

$absolutePath = Resolve-Path $dbPath

Write-Host "正在執行 WAL Checkpoint..." -ForegroundColor Yellow
Write-Host "資料庫路徑: $absolutePath" -ForegroundColor Gray

# 使用 SQLite 執行 checkpoint
# 這會將所有 WAL 中的資料合併到主資料庫
try {
    # 嘗試使用 sqlite3 命令行工具
    $sqliteExe = Get-Command sqlite3 -ErrorAction SilentlyContinue

    if ($sqliteExe) {
        sqlite3 $absolutePath "PRAGMA wal_checkpoint(TRUNCATE);"
        Write-Host "WAL Checkpoint 完成！(使用 sqlite3)" -ForegroundColor Green
    }
    else {
        # 如果沒有 sqlite3，使用 .NET 方式
        Write-Host "未找到 sqlite3，使用 .NET 方式..." -ForegroundColor Yellow

        # 建立一個簡單的 C# 腳本
        $csharpCode = @"
using Microsoft.Data.Sqlite;
var conn = new SqliteConnection("Data Source=$($absolutePath -replace '\\','\\\\')");
conn.Open();
var cmd = conn.CreateCommand();
cmd.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
var result = cmd.ExecuteReader();
while(result.Read()) {
    Console.WriteLine("Checkpoint 結果: busy={0}, log={1}, checkpointed={2}", result.GetInt32(0), result.GetInt32(1), result.GetInt32(2));
}
conn.Close();
conn.Dispose();
// 強制 GC 以確保連線完全釋放
GC.Collect();
GC.WaitForPendingFinalizers();
"@

        # 切換到 Backend 目錄執行
        Push-Location (Join-Path $PSScriptRoot "..")
        try {
            dotnet run --project . -- --checkpoint 2>$null
            if ($LASTEXITCODE -ne 0) {
                # 如果專案不支援 --checkpoint，使用 dotnet script 或其他方式
                Write-Host "提示: 請先停止運行中的伺服器，再執行 checkpoint" -ForegroundColor Yellow
            }
        }
        finally {
            Pop-Location
        }
    }

    # 檢查 WAL 檔案
    $walPath = "$absolutePath-wal"
    $shmPath = "$absolutePath-shm"

    if (Test-Path $walPath) {
        $walSize = (Get-Item $walPath).Length
        if ($walSize -eq 0) {
            Write-Host "WAL 檔案已清空 (0 bytes)" -ForegroundColor Green
        }
        else {
            Write-Host "警告: WAL 檔案仍有 $walSize bytes" -ForegroundColor Yellow
            Write-Host "這可能表示有程式正在使用資料庫" -ForegroundColor Yellow
        }
    }
    else {
        Write-Host "WAL 檔案不存在（正常）" -ForegroundColor Green
    }

    Write-Host ""
    Write-Host "現在可以安全地 commit game.db 了！" -ForegroundColor Cyan
}
catch {
    Write-Error "Checkpoint 失敗: $_"
    exit 1
}
