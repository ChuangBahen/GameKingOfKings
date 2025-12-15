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
import { gameHub, type StatsUpdate } from './services/gameHub';

const authStore = useAuthStore();
const playerStore = usePlayerStore();
const commandInput = ref('');
const commandHistory = ref<string[]>([]);
const historyIndex = ref(-1);
const messages = ref<{ user: string; content: string; timestamp: Date }[]>([]);
const inputRef = ref<HTMLInputElement | null>(null);

const connectSignalR = async () => {
  if (!authStore.isAuthenticated) return;

  try {
    await gameHub.start();
    console.log('SignalR Connected');
    playerStore.setConnected(true);

    // Register message handler
    gameHub.onReceiveMessage((user: string, message: string) => {
      messages.value.push({
        user,
        content: message,
        timestamp: new Date()
      });

      // Parse combat-related messages
      parseCombatMessage(user, message);

      // Scroll to bottom after message
      nextTick(() => {
        const terminal = document.getElementById('terminal-output');
        if (terminal) {
          terminal.scrollTop = terminal.scrollHeight;
        }
      });
    });

    // Register stats update handler
    gameHub.onUpdateStats((stats: StatsUpdate) => {
      if (stats.currentHp !== undefined) {
        playerStore.updateHp(stats.currentHp, stats.maxHp);
      }
      if (stats.monsterHp !== undefined && stats.monsterMaxHp !== undefined) {
        playerStore.updateMonsterHp(stats.monsterHp, stats.monsterMaxHp);
      }
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

const parseCombatMessage = (user: string, message: string) => {
  // 偵測戰鬥開始 (中英文訊息都支援)
  if (message.includes('Combat started with') || message.includes('開始與')) {
    const match = message.match(/Combat started with (.+?)!/) || message.match(/開始與 (.+?) 戰鬥/);
    if (match) {
      const monsterName = match[1];
      const hpMatch = message.match(/HP: (\d+)\/(\d+)/) || message.match(/生命值: (\d+)\/(\d+)/);
      if (hpMatch) {
        playerStore.startCombat(monsterName, parseInt(hpMatch[1]), parseInt(hpMatch[2]));
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
      commandInput.value = commandHistory.value[historyIndex.value];
    }
  } else if (e.key === 'ArrowDown') {
    e.preventDefault();
    if (historyIndex.value < commandHistory.value.length - 1) {
      historyIndex.value++;
      commandInput.value = commandHistory.value[historyIndex.value];
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
</script>

<template>
  <div class="min-h-screen bg-gray-950 text-white font-sans">
    <!-- Login Screen -->
    <Login v-if="!authStore.isAuthenticated" />

    <!-- Game Screen (3-Column Layout) -->
    <div v-else class="h-screen p-4 flex gap-4 overflow-hidden bg-gradient-to-br from-gray-900 via-gray-950 to-black">
      <!-- Left Panel: Status (20%) -->
      <div class="w-1/5 flex flex-col gap-4">
        <StatusPanel class="flex-1 shadow-lg" />
        <button
          @click="handleLogout"
          class="w-full py-2 bg-red-600 rounded hover:bg-red-500 transition-colors font-bold"
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
