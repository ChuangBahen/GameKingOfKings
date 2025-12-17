import * as signalR from "@microsoft/signalr";
import type { PlayerFullStats, InventoryData, MapData, SkillsData, MessageType } from '../types/game';

// The URL should match the backend Hub URL
// URL 應與後端 Hub URL 相符
const HUB_URL = "http://localhost:5000/gameHub";

export interface StatsUpdate {
    currentHp: number;
    maxHp: number;
    monsterHp?: number;
    monsterMaxHp?: number;
}

export interface ClassOption {
    type: number;
    name: string;
    description: string;
    stats: string;
}

export interface ClassSelectionData {
    classes?: ClassOption[];
    Classes?: ClassOption[];  // 後端使用大寫 C
}

export interface CharacterCreatedData {
    className: string;
}

// Extended message with type classification
export interface TypedMessage {
    user: string;
    message: string;
    messageType: MessageType;
}

class GameHubService {
    private connection: signalR.HubConnection | null = null;
    private messageCallbacks: ((user: string, message: string) => void)[] = [];
    private typedMessageCallbacks: ((data: TypedMessage) => void)[] = [];
    private statsCallbacks: ((stats: StatsUpdate) => void)[] = [];
    private classSelectionCallbacks: ((data: ClassSelectionData) => void)[] = [];
    private characterCreatedCallbacks: ((data: CharacterCreatedData) => void)[] = [];
    private fullStatsCallbacks: ((data: PlayerFullStats) => void)[] = [];
    private inventoryCallbacks: ((data: InventoryData) => void)[] = [];
    private mapCallbacks: ((data: MapData) => void)[] = [];
    private skillsCallbacks: ((data: SkillsData) => void)[] = [];

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

        // 註冊合併的 message handler（避免重複調用）
        // 後端 ReceiveMessage 發送 3 參數：user, message, messageType
        this.connection.on("ReceiveMessage", (user: string, message: string, messageType?: string) => {
            // 調用所有已註冊的 typed message callbacks
            this.typedMessageCallbacks.forEach(cb => {
                cb({ user, message, messageType: (messageType as import('../types/game').MessageType) || 'general' });
            });
        });

        // 其他事件 handlers
        this.statsCallbacks.forEach(cb => {
            this.connection?.on("UpdateStats", cb);
        });
        this.classSelectionCallbacks.forEach(cb => {
            this.connection?.on("RequireClassSelection", cb);
        });
        this.characterCreatedCallbacks.forEach(cb => {
            this.connection?.on("CharacterCreated", cb);
        });
        this.fullStatsCallbacks.forEach(cb => {
            this.connection?.on("FullStatsUpdate", cb);
        });
        this.inventoryCallbacks.forEach(cb => {
            this.connection?.on("InventoryUpdate", cb);
        });
        this.mapCallbacks.forEach(cb => {
            this.connection?.on("MapUpdate", cb);
        });
        this.skillsCallbacks.forEach(cb => {
            this.connection?.on("SkillsUpdate", cb);
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

    // Listen for class selection requirement
    // 監聽職業選擇要求
    public onRequireClassSelection(callback: (data: ClassSelectionData) => void) {
        this.classSelectionCallbacks.push(callback);
        this.connection?.on("RequireClassSelection", callback);
    }

    // Listen for character created event
    // 監聽角色建立事件
    public onCharacterCreated(callback: (data: CharacterCreatedData) => void) {
        this.characterCreatedCallbacks.push(callback);
        this.connection?.on("CharacterCreated", callback);
    }

    // Listen for typed messages (with message type classification)
    // 監聽類型化訊息（帶訊息分類）
    public onReceiveTypedMessage(callback: (data: TypedMessage) => void) {
        this.typedMessageCallbacks.push(callback);
        this.connection?.on("ReceiveTypedMessage", callback);
    }

    // Listen for full stats update
    // 監聽完整狀態更新
    public onFullStatsUpdate(callback: (data: PlayerFullStats) => void) {
        this.fullStatsCallbacks.push(callback);
        this.connection?.on("FullStatsUpdate", callback);
    }

    // Listen for inventory update
    // 監聽背包更新
    public onInventoryUpdate(callback: (data: InventoryData) => void) {
        this.inventoryCallbacks.push(callback);
        this.connection?.on("InventoryUpdate", callback);
    }

    // Listen for map update
    // 監聽地圖更新
    public onMapUpdate(callback: (data: MapData) => void) {
        this.mapCallbacks.push(callback);
        this.connection?.on("MapUpdate", callback);
    }

    // Listen for skills update
    // 監聽技能更新
    public onSkillsUpdate(callback: (data: SkillsData) => void) {
        this.skillsCallbacks.push(callback);
        this.connection?.on("SkillsUpdate", callback);
    }

    // Create character with selected class
    // 建立角色（選擇職業）
    public async createCharacter(classType: number) {
        try {
            await this.connection?.invoke("CreateCharacter", classType);
        } catch (err) {
            console.error("CreateCharacter Error: ", err);
        }
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
