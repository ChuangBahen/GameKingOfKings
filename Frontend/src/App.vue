<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useAuthStore } from './stores/auth';
import Login from './components/Login.vue';
import GameTerminal from './components/GameTerminal.vue';
import StatusPanel from './components/StatusPanel.vue';
import CombatView from './components/CombatView.vue';
import InventoryPanel from './components/InventoryPanel.vue';
import MiniMap from './components/MiniMap.vue';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

const authStore = useAuthStore();
const connection = ref<HubConnection | null>(null);

const connectSignalR = async () => {
  if (!authStore.isAuthenticated) return;

  const newConnection = new HubConnectionBuilder()
    .withUrl('http://localhost:5000/gameHub', {
      accessTokenFactory: () => authStore.token
    })
    .withAutomaticReconnect()
    .build();

  newConnection.on('ReceiveMessage', (user: string, message: string) => {
    // Handle messages if needed, or pass to GameTerminal via store/event
    console.log(`${user}: ${message}`);
  });

  try {
    await newConnection.start();
    console.log('SignalR Connected');
    connection.value = newConnection;
  } catch (err) {
    console.error('SignalR Connection Error: ', err);
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
    connection.value?.stop();
    connection.value = null;
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
    <div v-else class="h-screen p-4 flex gap-4 overflow-hidden bg-[url('https://raw.githubusercontent.com/gist/Harry/placeholder/bg_dark.jpg')] bg-cover bg-center">
      <!-- Left Panel: Status (20%) -->
      <div class="w-1/5 flex flex-col gap-4">
        <StatusPanel class="flex-1 shadow-lg" />
        <button 
          @click="handleLogout"
          class="w-full py-2 bg-red-600 rounded hover:bg-red-500 transition-colors font-bold"
        >
          Logout
        </button>
      </div>

      <!-- Center Panel: Main Game (50%) -->
      <div class="w-1/2 flex flex-col gap-4">
        <!-- Top: Visual Scene / Combat -->
        <div class="h-3/5 shadow-2xl">
          <CombatView />
        </div>
        
        <!-- Bottom: Terminal & Input -->
        <div class="h-2/5 flex flex-col gap-2 bg-black/80 backdrop-blur rounded-xl border border-gray-700 p-4 shadow-lg">
          <GameTerminal class="flex-1" />
          <!-- Input is likely inside GameTerminal or handled there, but original had it here. 
               I'll leave it to GameTerminal or add it if needed. 
               Original had input here. I'll add it back. -->
          <input type="text" 
                 placeholder="Enter command..." 
                 class="w-full bg-gray-900/50 border border-gray-600 rounded p-3 text-white focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500 transition-all font-mono" 
                 autofocus />
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
