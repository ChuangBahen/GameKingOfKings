import * as signalR from "@microsoft/signalr";

// The URL should match the backend Hub URL
// URL 應與後端 Hub URL 相符
const HUB_URL = "http://localhost:5000/gameHub";

export interface StatsUpdate {
    currentHp: number;
    maxHp: number;
    monsterHp?: number;
    monsterMaxHp?: number;
}

class GameHubService {
    private connection: signalR.HubConnection | null = null;
    private messageCallbacks: ((user: string, message: string) => void)[] = [];
    private statsCallbacks: ((stats: StatsUpdate) => void)[] = [];

    /**
     * 初始化 SignalR 連線（帶 JWT Token）
     */
    private initConnection() {
        const token = localStorage.getItem('token');

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, {
                accessTokenFactory: () => token || ''
            })
            .withAutomaticReconnect()
            .build();

        // 重新註冊已有的 callbacks
        this.messageCallbacks.forEach(cb => {
            this.connection?.on("ReceiveMessage", cb);
        });
        this.statsCallbacks.forEach(cb => {
            this.connection?.on("UpdateStats", cb);
        });
    }

    // Start the connection
    // 啟動連線
    public async start() {
        try {
            // 每次啟動時重新初始化連線（確保使用最新的 token）
            this.initConnection();

            if (!this.connection) {
                throw new Error("連線初始化失敗");
            }

            await this.connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
            setTimeout(() => this.start(), 5000);
        }
    }

    // Join the game (不再需要傳 username，後端從 JWT 取得)
    // 加入遊戲
    public async joinGame(_username?: string) {
        try {
            await this.connection?.invoke("JoinGame");
        } catch (err) {
            console.error("JoinGame Error: ", err);
        }
    }

    // Send a command
    // 發送指令
    public async sendCommand(command: string) {
        try {
            await this.connection?.invoke("SendCommand", command);
        } catch (err) {
            console.error("SendCommand Error: ", err);
        }
    }

    // Listen for messages
    // 監聽訊息
    public onReceiveMessage(callback: (user: string, message: string) => void) {
        this.messageCallbacks.push(callback);
        this.connection?.on("ReceiveMessage", callback);
    }

    // Listen for stats updates
    // 監聯狀態更新
    public onUpdateStats(callback: (stats: StatsUpdate) => void) {
        this.statsCallbacks.push(callback);
        this.connection?.on("UpdateStats", callback);
    }

    // Send a message (chat)
    // 發送訊息（聊天）
    public async sendMessage(user: string, message: string) {
        await this.connection?.invoke("SendMessage", user, message);
    }

    // Check if connected
    // 檢查是否已連線
    public isConnected(): boolean {
        return this.connection?.state === signalR.HubConnectionState.Connected;
    }

    // Get connection state
    // 取得連線狀態
    public getState(): signalR.HubConnectionState {
        return this.connection?.state ?? signalR.HubConnectionState.Disconnected;
    }

    // Stop connection
    // 停止連線
    public async stop() {
        await this.connection?.stop();
    }
}

export const gameHub = new GameHubService();
