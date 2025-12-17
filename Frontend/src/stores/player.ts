import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { PlayerFullStats, InventoryData, MapData, SkillsData, GameMessage, MessageType } from '../types/game'

// Define the Player interface based on the backend model
export interface Player {
    name: string;
    currentHp: number;
    maxHp: number;
    currentMp: number;
    maxMp: number;
    level: number;
    exp: number;
    className?: string;
}

// Combat state interface
export interface CombatState {
    inCombat: boolean;
    monsterName: string;
    monsterHp: number;
    monsterMaxHp: number;
    monsterEmoji: string;
}

const MAX_MESSAGES_PER_TYPE = 100;

export const usePlayerStore = defineStore('player', () => {
    // Basic State
    const player = ref<Player | null>(null);
    const isConnected = ref(false);
    const combat = ref<CombatState>({
        inCombat: false,
        monsterName: '',
        monsterHp: 0,
        monsterMaxHp: 0,
        monsterEmoji: '🐺'
    });

    // Extended State for UI optimization
    const fullStats = ref<PlayerFullStats | null>(null);
    const inventory = ref<InventoryData | null>(null);
    const mapData = ref<MapData | null>(null);
    const skills = ref<SkillsData | null>(null);
    const messages = ref<GameMessage[]>([]);
    const unreadCombatCount = ref(0);
    const unreadGeneralCount = ref(0);
    const activeMessageTab = ref<MessageType>('general');

    // Computed - Basic
    const hpPercent = computed(() => {
        if (fullStats.value) {
            return (fullStats.value.currentHp / fullStats.value.maxHp) * 100;
        }
        if (!player.value) return 0;
        return (player.value.currentHp / player.value.maxHp) * 100;
    });

    const mpPercent = computed(() => {
        if (fullStats.value) {
            return (fullStats.value.currentMp / fullStats.value.maxMp) * 100;
        }
        if (!player.value) return 0;
        return (player.value.currentMp / player.value.maxMp) * 100;
    });

    const expPercent = computed(() => {
        if (fullStats.value) {
            return (fullStats.value.exp / fullStats.value.expRequired) * 100;
        }
        if (!player.value) return 0;
        const expRequired = player.value.level * 100;
        return (player.value.exp / expRequired) * 100;
    });

    const monsterHpPercent = computed(() => {
        if (!combat.value.inCombat || combat.value.monsterMaxHp === 0) return 0;
        return (combat.value.monsterHp / combat.value.monsterMaxHp) * 100;
    });

    // Computed - Message filtering
    const combatMessages = computed(() =>
        messages.value.filter(m => m.type === 'combat').slice(-50)
    );

    const generalMessages = computed(() =>
        messages.value.filter(m => m.type === 'general' || m.type === 'system').slice(-50)
    );

    const allMessages = computed(() => messages.value.slice(-100));

    // Computed - Inventory
    const equippedItems = computed(() =>
        inventory.value?.items.filter(i => i.isEquipped) ?? []
    );

    const backpackItems = computed(() =>
        inventory.value?.items.filter(i => !i.isEquipped) ?? []
    );

    // Computed - Skills
    const learnedSkills = computed(() => skills.value?.learnedSkills ?? []);
    const lockedSkills = computed(() => skills.value?.lockedSkills ?? []);

    // Actions - Basic
    function setPlayer(data: Player) {
        player.value = data;
    }

    function updateHp(current: number, max: number) {
        if (player.value) {
            player.value.currentHp = current;
            player.value.maxHp = max;
        }
    }

    function updateMp(current: number, max: number) {
        if (player.value) {
            player.value.currentMp = current;
            player.value.maxMp = max;
        }
    }

    function updateExp(exp: number) {
        if (player.value) {
            player.value.exp = exp;
        }
    }

    function levelUp(newLevel: number) {
        if (player.value) {
            player.value.level = newLevel;
        }
    }

    function startCombat(monsterName: string, monsterHp: number, monsterMaxHp: number) {
        combat.value = {
            inCombat: true,
            monsterName,
            monsterHp,
            monsterMaxHp,
            monsterEmoji: getMonsterEmoji(monsterName)
        };
    }

    function updateMonsterHp(hp: number, maxHp?: number) {
        combat.value.monsterHp = hp;
        if (maxHp !== undefined) {
            combat.value.monsterMaxHp = maxHp;
        }
    }

    function endCombat() {
        combat.value = {
            inCombat: false,
            monsterName: '',
            monsterHp: 0,
            monsterMaxHp: 0,
            monsterEmoji: '🐺'
        };
    }

    function setConnected(connected: boolean) {
        isConnected.value = connected;
    }

    // Actions - Extended State Setters
    function setFullStats(data: PlayerFullStats) {
        fullStats.value = data;
        // Sync with basic player data
        if (player.value) {
            player.value.currentHp = data.currentHp;
            player.value.maxHp = data.maxHp;
            player.value.currentMp = data.currentMp;
            player.value.maxMp = data.maxMp;
            player.value.level = data.level;
            player.value.exp = data.exp;
            player.value.className = data.className;
        }
    }

    function setInventory(data: InventoryData) {
        inventory.value = data;
    }

    function setMapData(data: MapData) {
        mapData.value = data;
    }

    function setSkills(data: SkillsData) {
        skills.value = data;
    }

    // Actions - Messages
    function addMessage(user: string, content: string, type: MessageType = 'general') {
        const message: GameMessage = {
            user,
            content,
            type,
            timestamp: new Date()
        };
        messages.value.push(message);

        // Trim messages if exceeding limit
        const typeMessages = messages.value.filter(m => m.type === type);
        if (typeMessages.length > MAX_MESSAGES_PER_TYPE) {
            const toRemove = typeMessages.slice(0, typeMessages.length - MAX_MESSAGES_PER_TYPE);
            messages.value = messages.value.filter(m => !toRemove.includes(m));
        }

        // Update unread counts
        if (type === 'combat' && activeMessageTab.value !== 'combat') {
            unreadCombatCount.value++;
        } else if ((type === 'general' || type === 'system') && activeMessageTab.value !== 'general') {
            unreadGeneralCount.value++;
        }
    }

    function setActiveMessageTab(tab: MessageType) {
        activeMessageTab.value = tab;
        if (tab === 'combat') {
            unreadCombatCount.value = 0;
        } else if (tab === 'general') {
            unreadGeneralCount.value = 0;
        }
    }

    function clearMessages() {
        messages.value = [];
        unreadCombatCount.value = 0;
        unreadGeneralCount.value = 0;
    }

    // Helper function to get monster emoji
    function getMonsterEmoji(name: string): string {
        const lowerName = name.toLowerCase();
        if (lowerName.includes('slime') || lowerName.includes('史萊姆')) return '🟢';
        if (lowerName.includes('rat') || lowerName.includes('老鼠')) return '🐀';
        if (lowerName.includes('wolf') || lowerName.includes('狼')) return '🐺';
        if (lowerName.includes('goblin') || lowerName.includes('哥布林')) return '👺';
        if (lowerName.includes('skeleton') || lowerName.includes('骷髏')) return '💀';
        if (lowerName.includes('zombie') || lowerName.includes('殭屍')) return '🧟';
        if (lowerName.includes('king') || lowerName.includes('王')) return '👑';
        return '👾';
    }

    return {
        // Basic State
        player,
        isConnected,
        combat,
        // Extended State
        fullStats,
        inventory,
        mapData,
        skills,
        messages,
        unreadCombatCount,
        unreadGeneralCount,
        activeMessageTab,
        // Computed - Basic
        hpPercent,
        mpPercent,
        expPercent,
        monsterHpPercent,
        // Computed - Messages
        combatMessages,
        generalMessages,
        allMessages,
        // Computed - Inventory
        equippedItems,
        backpackItems,
        // Computed - Skills
        learnedSkills,
        lockedSkills,
        // Actions - Basic
        setPlayer,
        updateHp,
        updateMp,
        updateExp,
        levelUp,
        startCombat,
        updateMonsterHp,
        endCombat,
        setConnected,
        // Actions - Extended
        setFullStats,
        setInventory,
        setMapData,
        setSkills,
        addMessage,
        setActiveMessageTab,
        clearMessages
    }
})
