<script setup lang="ts">
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()
</script>

<template>
  <div class="bg-gray-800/90 backdrop-blur p-4 rounded-xl border border-gray-700 text-white h-full flex flex-col">
    <h2 class="text-xl font-bold mb-4 text-yellow-400 border-b border-gray-600 pb-2">
      ⚔️ 狀態
    </h2>

    <div v-if="playerStore.player" class="flex-1 space-y-4">
      <!-- Name & Level -->
      <div class="bg-gray-900/50 rounded-lg p-3">
        <div class="text-lg font-bold text-cyan-400">{{ playerStore.player.name }}</div>
        <div class="text-sm text-gray-400">
          等級 <span class="text-yellow-400 font-bold">{{ playerStore.player.level }}</span>
          <span v-if="playerStore.player.className" class="ml-2">• {{ playerStore.player.className }}</span>
        </div>
      </div>

      <!-- HP Bar -->
      <div class="space-y-1">
        <div class="flex justify-between text-sm">
          <span class="text-red-400 font-medium">❤️ 生命值</span>
          <span class="text-gray-300">{{ playerStore.player.currentHp }} / {{ playerStore.player.maxHp }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-4 overflow-hidden">
          <div
            class="h-full rounded-full transition-all duration-300 bg-gradient-to-r from-red-600 to-red-400"
            :style="{ width: playerStore.hpPercent + '%' }"
          ></div>
        </div>
      </div>

      <!-- MP Bar -->
      <div class="space-y-1">
        <div class="flex justify-between text-sm">
          <span class="text-blue-400 font-medium">💧 魔力值</span>
          <span class="text-gray-300">{{ playerStore.player.currentMp }} / {{ playerStore.player.maxMp }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-4 overflow-hidden">
          <div
            class="h-full rounded-full transition-all duration-300 bg-gradient-to-r from-blue-600 to-blue-400"
            :style="{ width: playerStore.mpPercent + '%' }"
          ></div>
        </div>
      </div>

      <!-- EXP Bar -->
      <div class="space-y-1">
        <div class="flex justify-between text-sm">
          <span class="text-yellow-400 font-medium">⭐ 經驗值</span>
          <span class="text-gray-300">{{ playerStore.player.exp }} / {{ playerStore.player.level * 100 }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-3 overflow-hidden">
          <div
            class="h-full rounded-full transition-all duration-300 bg-gradient-to-r from-yellow-600 to-yellow-400"
            :style="{ width: playerStore.expPercent + '%' }"
          ></div>
        </div>
      </div>

      <!-- Combat Status -->
      <div v-if="playerStore.combat.inCombat" class="bg-red-900/30 border border-red-700 rounded-lg p-3 mt-4">
        <div class="text-red-400 font-bold text-sm flex items-center gap-2">
          <span class="animate-pulse">⚔️</span>
          戰鬥中
        </div>
        <div class="text-gray-300 text-sm mt-1">
          對戰: <span class="text-red-300">{{ playerStore.combat.monsterName }}</span>
        </div>
      </div>

      <!-- Connection Status -->
      <div class="mt-auto pt-4 border-t border-gray-700">
        <div class="flex items-center gap-2 text-sm">
          <span
            class="w-2 h-2 rounded-full"
            :class="playerStore.isConnected ? 'bg-green-500' : 'bg-red-500'"
          ></span>
          <span class="text-gray-400">
            {{ playerStore.isConnected ? '已連線' : '已斷線' }}
          </span>
        </div>
      </div>
    </div>

    <div v-else class="flex-1 flex items-center justify-center">
      <div class="text-gray-500 italic text-center">
        <div class="text-4xl mb-2 opacity-30">🎮</div>
        <div>連線中...</div>
      </div>
    </div>
  </div>
</template>
