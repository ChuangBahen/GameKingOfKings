<script setup lang="ts">
import { onMounted, ref, watch, nextTick } from 'vue';
import { useAuthStore } from './stores/auth';
import { usePlayerStore } from './stores/player';
import Login from './components/Login.vue';
import GameTerminal from './components/GameTerminal.vue';
import StatusPanel from './components/StatusPanel.vue';
import CombatView from './components/CombatView.vue';
import InventoryPanel from './components/InventoryPanel.vue';
import MiniMap from './components/MiniMap.vue';
import SkillPanel from './components/SkillPanel.vue';
import { gameHub, type StatsUpdate, type ClassOption, type ClassSelectionData, type TypedMessage } from './services/gameHub';
import type { PlayerFullStats, InventoryData, MapData, SkillsData } from './types/game';

const authStore = useAuthStore();
const playerStore = usePlayerStore();
const commandInput = ref('');
const commandHistory = ref<string[]>([]);
const historyIndex = ref(-1);
const messages = ref<{ user: string; content: string; timestamp: Date }[]>([]);
const inputRef = ref<HTMLInputElement | null>(null);

// 職業選擇相關
const showClassSelection = ref(false);
const classOptions = ref<ClassOption[]>([]);
const selectedClass = ref<number | null>(null);
const isCreatingCharacter = ref(false);

const connectSignalR = async () => {
  if (!authStore.isAuthenticated) return;

  try {
    await gameHub.start();
    console.log('SignalR Connected');
    playerStore.setConnected(true);

    // Register stats update handler
    gameHub.onUpdateStats((stats: StatsUpdate) => {
      if (stats.currentHp !== undefined) {
        playerStore.updateHp(stats.currentHp, stats.maxHp);
      }
      if (stats.monsterHp !== undefined && stats.monsterMaxHp !== undefined) {
        playerStore.updateMonsterHp(stats.monsterHp, stats.monsterMaxHp);
      }
    });

    // Register class selection handler
    gameHub.onRequireClassSelection((data: ClassSelectionData) => {
      console.log('Class selection required:', data);
      // 後端使用大寫 Classes，前端介面使用小寫 classes
      const classes = data.classes || data.Classes;
      if (classes && classes.length > 0) {
        classOptions.value = classes;
        showClassSelection.value = true;
      } else {
        console.error('Invalid class selection data:', data);
      }
    });

    // Register character created handler
    gameHub.onCharacterCreated(() => {
      console.log('Character created');
      showClassSelection.value = false;
      isCreatingCharacter.value = false;
      selectedClass.value = null;
    });

    // Register typed message handler (new - with message type)
    gameHub.onReceiveTypedMessage((data: TypedMessage) => {
      playerStore.addMessage(data.user, data.message, data.messageType);

      // Also add to legacy messages array for backward compatibility
      messages.value.push({
        user: data.user,
        content: data.message,
        timestamp: new Date()
      });

      // Parse combat-related messages
      parseCombatMessage(data.user, data.message);

      // Scroll to bottom after message
      nextTick(() => {
        const terminal = document.getElementById('terminal-output');
        if (terminal) {
          terminal.scrollTop = terminal.scrollHeight;
        }
      });
    });

    // Register full stats update handler
    gameHub.onFullStatsUpdate((data: PlayerFullStats) => {
      console.log('Full stats update:', data);
      playerStore.setFullStats(data);
    });

    // Register inventory update handler
    gameHub.onInventoryUpdate((data: InventoryData) => {
      console.log('Inventory update:', data);
      playerStore.setInventory(data);
    });

    // Register map update handler
    gameHub.onMapUpdate((data: MapData) => {
      console.log('Map update:', data);
      playerStore.setMapData(data);
    });

    // Register skills update handler
    gameHub.onSkillsUpdate((data: SkillsData) => {
      console.log('Skills update:', data);
      playerStore.setSkills(data);
    });

    // Join the game with username
    await gameHub.joinGame(authStore.username);

    // Set default player data
    playerStore.setPlayer({
      name: authStore.username,
      currentHp: 100,
      maxHp: 100,
      currentMp: 50,
      maxMp: 50,
      level: 1,
      exp: 0
    });

  } catch (err) {
    console.error('SignalR Connection Error: ', err);
    playerStore.setConnected(false);
  }
};

const parseCombatMessage = (_user: string, message: string) => {
  // 偵測戰鬥開始 (中英文訊息都支援)
  if (message.includes('Combat started with') || message.includes('開始與')) {
    const match = message.match(/Combat started with (.+?)!/) || message.match(/開始與 (.+?) 戰鬥/);
    if (match) {
      const monsterName = match[1] ?? 'Unknown';
      const hpMatch = message.match(/HP: (\d+)\/(\d+)/) || message.match(/生命值: (\d+)\/(\d+)/);
      if (hpMatch) {
        playerStore.startCombat(monsterName, parseInt(hpMatch[1] ?? '100'), parseInt(hpMatch[2] ?? '100'));
      } else {
        playerStore.startCombat(monsterName, 100, 100);
      }
    }
  }

  // 偵測戰鬥結束 (中英文訊息都支援)
  if (message.includes('defeated!') || message.includes('打倒了') ||
      message.includes('have died') || message.includes('陣亡') ||
      message.includes('fled from combat') || message.includes('逃離了戰鬥')) {
    playerStore.endCombat();
  }
};

const handleCommand = async () => {
  const cmd = commandInput.value.trim();
  if (!cmd) return;

  // Add to history
  commandHistory.value.push(cmd);
  historyIndex.value = commandHistory.value.length;

  // Add command to messages
  messages.value.push({
    user: 'You',
    content: cmd,
    timestamp: new Date()
  });

  // Send command to server
  await gameHub.sendCommand(cmd);

  // Clear input
  commandInput.value = '';
};

const handleKeyDown = (e: KeyboardEvent) => {
  if (e.key === 'ArrowUp') {
    e.preventDefault();
    if (historyIndex.value > 0) {
      historyIndex.value--;
      commandInput.value = commandHistory.value[historyIndex.value] ?? '';
    }
  } else if (e.key === 'ArrowDown') {
    e.preventDefault();
    if (historyIndex.value < commandHistory.value.length - 1) {
      historyIndex.value++;
      commandInput.value = commandHistory.value[historyIndex.value] ?? '';
    } else {
      historyIndex.value = commandHistory.value.length;
      commandInput.value = '';
    }
  }
};

onMounted(() => {
  if (authStore.isAuthenticated) {
    connectSignalR();
  }
});

watch(() => authStore.isAuthenticated, (newValue) => {
  if (newValue) {
    connectSignalR();
  } else {
    gameHub.stop();
    playerStore.setConnected(false);
    messages.value = [];
  }
});

const handleLogout = () => {
  authStore.logout();
};

const handleClassSelect = async () => {
  if (selectedClass.value === null) return;
  isCreatingCharacter.value = true;
  await gameHub.createCharacter(selectedClass.value);
};
</script>

<template>
  <div class="min-h-screen bg-gray-950 text-white font-sans">
    <!-- Login Screen -->
    <Login v-if="!authStore.isAuthenticated" />

    <!-- Class Selection Modal -->
    <div v-else-if="showClassSelection" class="fixed inset-0 bg-black/90 flex items-center justify-center z-50">
      <div class="w-full max-w-4xl p-8 space-y-8 bg-gray-800 rounded-xl shadow-2xl">
        <div class="text-center">
          <h2 class="text-4xl font-bold text-yellow-500 mb-2">選擇你的職業</h2>
          <p class="text-gray-400">選擇一個職業開始你的冒險</p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div
            v-for="cls in classOptions"
            :key="cls.type"
            @click="selectedClass = cls.type"
            :class="[
              'p-6 rounded-xl cursor-pointer transition-all border-2 transform hover:scale-105',
              selectedClass === cls.type
                ? 'border-yellow-500 bg-gray-700 shadow-lg shadow-yellow-500/20'
                : 'border-gray-600 bg-gray-750 hover:border-gray-500'
            ]"
          >
            <div class="text-center mb-4">
              <span class="text-5xl">
                {{ cls.type === 0 ? '&#x2694;&#xFE0F;' : cls.type === 1 ? '&#x1F9D9;' : '&#x2728;' }}
              </span>
            </div>
            <h3 class="text-2xl font-bold text-yellow-400 mb-3 text-center">{{ cls.name }}</h3>
            <p class="text-sm text-gray-300 mb-4 text-center">{{ cls.description }}</p>
            <div class="bg-black/30 rounded-lg p-3">
              <p class="text-xs text-gray-400 font-mono text-center">{{ cls.stats }}</p>
            </div>
          </div>
        </div>

        <div class="flex justify-center gap-4">
          <button
            @click="handleLogout"
            class="px-8 py-4 text-xl font-bold text-gray-300 bg-gray-600 rounded-lg hover:bg-gray-500 transition-all"
          >
            返回登入
          </button>
          <button
            @click="handleClassSelect"
            :disabled="selectedClass === null || isCreatingCharacter"
            class="px-12 py-4 text-xl font-bold text-gray-900 bg-yellow-500 rounded-lg hover:bg-yellow-400 transition-all disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:bg-yellow-500 transform hover:scale-105"
          >
            {{ isCreatingCharacter ? '建立中...' : '確認選擇' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Game Screen (3-Column Layout) -->
    <div v-else class="h-screen p-4 flex gap-4 overflow-hidden bg-gradient-to-br from-gray-900 via-gray-950 to-black">
      <!-- Left Panel: Status & Skills (20%) -->
      <div class="w-1/5 flex flex-col gap-4">
        <StatusPanel class="h-1/2 shadow-lg" />
        <SkillPanel class="h-1/2 shadow-lg" />
        <button
          @click="handleLogout"
          class="w-full py-2 bg-red-600 rounded hover:bg-red-500 transition-colors font-bold shrink-0"
        >
          登出
        </button>
      </div>

      <!-- Center Panel: Main Game (50%) -->
      <div class="w-1/2 flex flex-col gap-4">
        <!-- Top: Visual Scene / Combat -->
        <div class="h-3/5 shadow-2xl">
          <CombatView :messages="messages" />
        </div>

        <!-- Bottom: Terminal & Input -->
        <div class="h-2/5 flex flex-col gap-2 bg-black/80 backdrop-blur rounded-xl border border-gray-700 p-4 shadow-lg">
          <GameTerminal :messages="messages" class="flex-1" />
          <!-- Command Input -->
          <form @submit.prevent="handleCommand" class="flex gap-2">
            <input
              ref="inputRef"
              v-model="commandInput"
              @keydown="handleKeyDown"
              type="text"
              placeholder="輸入指令... (輸入 'help' 查看指令列表)"
              class="flex-1 bg-gray-900/50 border border-gray-600 rounded p-3 text-white focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-all font-mono"
              autofocus
            />
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 hover:bg-blue-500 rounded font-bold transition-colors"
            >
              發送
            </button>
          </form>
        </div>
      </div>

      <!-- Right Panel: Utility (30%) -->
      <div class="w-[30%] flex flex-col gap-4">
        <!-- Top: Mini Map -->
        <div class="h-1/3 shadow-lg">
          <MiniMap />
        </div>

        <!-- Bottom: Inventory -->
        <div class="h-2/3 shadow-lg">
          <InventoryPanel />
        </div>
      </div>
    </div>
  </div>
</template>

<style>
body {
  background-color: #0f172a;
  margin: 0;
  overflow: hidden;
}

/* Global scrollbar styling */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.1);
}
::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.2);
  border-radius: 3px;
}
::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.3);
}
</style>
