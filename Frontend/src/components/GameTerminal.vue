<script setup lang="ts">
import { computed, nextTick, watch } from 'vue'

interface Message {
  user: string;
  content: string;
  timestamp: Date;
}

const props = defineProps<{
  messages: Message[];
}>();

// Filter messages for terminal (exclude combat-only messages if needed)
const terminalMessages = computed(() => {
  return props.messages;
});

// Auto-scroll when new messages arrive
watch(() => props.messages.length, () => {
  nextTick(() => {
    const terminal = document.getElementById('terminal-output');
    if (terminal) {
      terminal.scrollTop = terminal.scrollHeight;
    }
  });
});

const formatTime = (date: Date) => {
  return date.toLocaleTimeString('zh-TW', { hour12: false });
};

const getUserColor = (user: string) => {
  switch (user) {
    case 'System': return 'text-yellow-400';
    case 'Game': return 'text-green-400';
    case 'Combat': return 'text-red-400';
    case 'You': return 'text-cyan-400';
    default: return 'text-gray-400';
  }
};

// 顯示名稱中文化
const getDisplayName = (user: string) => {
  switch (user) {
    case 'System': return '系統';
    case 'Game': return '遊戲';
    case 'Combat': return '戰鬥';
    case 'You': return '你';
    default: return user;
  }
};
</script>

<template>
  <div
    id="terminal-output"
    class="bg-gray-900/50 text-gray-300 font-mono p-4 rounded-lg h-full overflow-y-auto text-sm"
  >
    <div
      v-for="(msg, index) in terminalMessages"
      :key="index"
      class="mb-2 animate-fade-in"
    >
      <span class="text-gray-500">[{{ formatTime(msg.timestamp) }}]</span>
      <span :class="getUserColor(msg.user)" class="font-bold ml-1">{{ getDisplayName(msg.user) }}:</span>
      <!-- Render HTML content from server -->
      <span class="ml-2" v-html="msg.content"></span>
    </div>

    <div v-if="terminalMessages.length === 0" class="text-gray-500 italic">
      等待遊戲資料...
    </div>
  </div>
</template>

<style scoped>
/* Custom scrollbar for terminal feel */
::-webkit-scrollbar {
  width: 8px;
}
::-webkit-scrollbar-track {
  background: #1a1b26;
}
::-webkit-scrollbar-thumb {
  background: #414868;
  border-radius: 4px;
}

.animate-fade-in {
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(5px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Style HTML content from server */
:deep(.text-yellow-400) { color: rgb(250 204 21); }
:deep(.text-yellow-300) { color: rgb(253 224 71); }
:deep(.text-green-400) { color: rgb(74 222 128); }
:deep(.text-green-300) { color: rgb(134 239 172); }
:deep(.text-red-400) { color: rgb(248 113 113); }
:deep(.text-red-300) { color: rgb(252 165 165); }
:deep(.text-red-500) { color: rgb(239 68 68); }
:deep(.text-blue-400) { color: rgb(96 165 250); }
:deep(.text-blue-300) { color: rgb(147 197 253); }
:deep(.text-cyan-400) { color: rgb(34 211 238); }
:deep(.text-orange-400) { color: rgb(251 146 60); }
:deep(.text-purple-400) { color: rgb(192 132 252); }
:deep(.text-gray-400) { color: rgb(156 163 175); }
:deep(.text-gray-300) { color: rgb(209 213 219); }
:deep(.font-bold) { font-weight: 700; }
:deep(.mt-2) { margin-top: 0.5rem; }
</style>
