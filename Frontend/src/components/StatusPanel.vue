<script setup lang="ts">
import { usePlayerStore } from '../stores/player'
import { computed } from 'vue'

const playerStore = usePlayerStore()

// Use fullStats if available, otherwise fall back to basic player data
const displayStats = computed(() => {
  if (playerStore.fullStats) {
    return {
      name: playerStore.fullStats.name,
      className: playerStore.fullStats.className,
      level: playerStore.fullStats.level,
      currentHp: playerStore.fullStats.currentHp,
      maxHp: playerStore.fullStats.maxHp,
      currentMp: playerStore.fullStats.currentMp,
      maxMp: playerStore.fullStats.maxMp,
      exp: playerStore.fullStats.exp,
      expRequired: playerStore.fullStats.expRequired,
      stats: playerStore.fullStats.stats,
      equipmentBonuses: playerStore.fullStats.equipmentBonuses,
      setBonuses: playerStore.fullStats.setBonuses || null,
      activeSets: playerStore.fullStats.activeSets || []
    }
  }
  if (playerStore.player) {
    return {
      name: playerStore.player.name,
      className: playerStore.player.className,
      level: playerStore.player.level,
      currentHp: playerStore.player.currentHp,
      maxHp: playerStore.player.maxHp,
      currentMp: playerStore.player.currentMp,
      maxMp: playerStore.player.maxMp,
      exp: playerStore.player.exp,
      expRequired: playerStore.player.level * 100,
      stats: null,
      equipmentBonuses: null,
      setBonuses: null,
      activeSets: []
    }
  }
  return null
})
</script>

<template>
  <div class="bg-gray-800/90 backdrop-blur p-4 rounded-xl border border-gray-700 text-white h-full flex flex-col overflow-y-auto">
    <h2 class="text-xl font-bold mb-4 text-yellow-400 border-b border-gray-600 pb-2">
      ⚔️ 狀態
    </h2>

    <div v-if="displayStats" class="flex-1 space-y-3">
      <!-- Name & Level -->
      <div class="bg-gray-900/50 rounded-lg p-3">
        <div class="text-lg font-bold text-cyan-400">{{ displayStats.name }}</div>
        <div class="text-sm text-gray-400">
          等級 <span class="text-yellow-400 font-bold">{{ displayStats.level }}</span>
          <span v-if="displayStats.className" class="ml-2">• {{ displayStats.className }}</span>
        </div>
      </div>

      <!-- HP Bar -->
      <div class="space-y-1">
        <div class="flex justify-between text-sm">
          <span class="text-red-400 font-medium">❤️ 生命值</span>
          <span class="text-gray-300">{{ displayStats.currentHp }} / {{ displayStats.maxHp }}</span>
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
          <span class="text-gray-300">{{ displayStats.currentMp }} / {{ displayStats.maxMp }}</span>
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
          <span class="text-gray-300">{{ displayStats.exp }} / {{ displayStats.expRequired }}</span>
        </div>
        <div class="w-full bg-gray-700 rounded-full h-3 overflow-hidden">
          <div
            class="h-full rounded-full transition-all duration-300 bg-gradient-to-r from-yellow-600 to-yellow-400"
            :style="{ width: playerStore.expPercent + '%' }"
          ></div>
        </div>
      </div>

      <!-- Character Stats (五維屬性) -->
      <div v-if="displayStats.stats" class="bg-gray-900/50 rounded-lg p-3">
        <div class="text-sm font-bold text-gray-400 mb-2">屬性</div>
        <div class="grid grid-cols-2 gap-2 text-xs">
          <div class="flex justify-between">
            <span class="text-orange-400">力量 STR</span>
            <span class="text-white">
              {{ displayStats.stats.str }}
              <span v-if="displayStats.equipmentBonuses?.str > 0" class="text-green-400">+{{ displayStats.equipmentBonuses.str }}</span>
              <span v-if="displayStats.setBonuses?.str > 0" class="text-blue-400">+{{ displayStats.setBonuses.str }}</span>
            </span>
          </div>
          <div class="flex justify-between">
            <span class="text-green-400">敏捷 DEX</span>
            <span class="text-white">
              {{ displayStats.stats.dex }}
              <span v-if="displayStats.equipmentBonuses?.dex > 0" class="text-green-400">+{{ displayStats.equipmentBonuses.dex }}</span>
              <span v-if="displayStats.setBonuses?.dex > 0" class="text-blue-400">+{{ displayStats.setBonuses.dex }}</span>
            </span>
          </div>
          <div class="flex justify-between">
            <span class="text-purple-400">智力 INT</span>
            <span class="text-white">
              {{ displayStats.stats.int }}
              <span v-if="displayStats.equipmentBonuses?.int > 0" class="text-green-400">+{{ displayStats.equipmentBonuses.int }}</span>
              <span v-if="displayStats.setBonuses?.int > 0" class="text-blue-400">+{{ displayStats.setBonuses.int }}</span>
            </span>
          </div>
          <div class="flex justify-between">
            <span class="text-blue-400">智慧 WIS</span>
            <span class="text-white">
              {{ displayStats.stats.wis }}
              <span v-if="displayStats.equipmentBonuses?.wis > 0" class="text-green-400">+{{ displayStats.equipmentBonuses.wis }}</span>
              <span v-if="displayStats.setBonuses?.wis > 0" class="text-blue-400">+{{ displayStats.setBonuses.wis }}</span>
            </span>
          </div>
          <div class="flex justify-between col-span-2">
            <span class="text-red-400">體質 CON</span>
            <span class="text-white">
              {{ displayStats.stats.con }}
              <span v-if="displayStats.equipmentBonuses?.con > 0" class="text-green-400">+{{ displayStats.equipmentBonuses.con }}</span>
              <span v-if="displayStats.setBonuses?.con > 0" class="text-blue-400">+{{ displayStats.setBonuses.con }}</span>
            </span>
          </div>
        </div>
      </div>

      <!-- Equipment & Set Bonuses -->
      <div v-if="(displayStats.equipmentBonuses && (displayStats.equipmentBonuses.atk > 0 || displayStats.equipmentBonuses.def > 0)) ||
                 (displayStats.setBonuses && (displayStats.setBonuses.atk > 0 || displayStats.setBonuses.def > 0))"
           class="bg-gray-900/50 rounded-lg p-3">
        <div class="text-sm font-bold text-gray-400 mb-2">戰鬥加成</div>
        <div class="grid grid-cols-2 gap-2 text-xs">
          <div v-if="(displayStats.equipmentBonuses?.atk || 0) > 0 || (displayStats.setBonuses?.atk || 0) > 0"
               class="flex justify-between">
            <span class="text-red-400">攻擊力</span>
            <span>
              <span v-if="displayStats.equipmentBonuses?.atk" class="text-green-400">+{{ displayStats.equipmentBonuses.atk }}</span>
              <span v-if="displayStats.setBonuses?.atk" class="text-blue-400 ml-1">+{{ displayStats.setBonuses.atk }}</span>
            </span>
          </div>
          <div v-if="(displayStats.equipmentBonuses?.def || 0) > 0 || (displayStats.setBonuses?.def || 0) > 0"
               class="flex justify-between">
            <span class="text-blue-400">防禦力</span>
            <span>
              <span v-if="displayStats.equipmentBonuses?.def" class="text-green-400">+{{ displayStats.equipmentBonuses.def }}</span>
              <span v-if="displayStats.setBonuses?.def" class="text-blue-400 ml-1">+{{ displayStats.setBonuses.def }}</span>
            </span>
          </div>
        </div>
      </div>

      <!-- Active Equipment Sets -->
      <div v-if="displayStats.activeSets && displayStats.activeSets.length > 0"
           class="bg-gray-900/50 rounded-lg p-3">
        <div class="text-sm font-bold text-purple-400 mb-2">✨ 套裝效果</div>
        <div v-for="set in displayStats.activeSets" :key="set.setName" class="mb-2 last:mb-0">
          <div class="text-xs text-blue-300 font-medium">
            {{ set.setName }} ({{ set.equippedPieces }}/{{ set.totalPieces }})
          </div>
          <div v-for="bonus in set.activeBonuses" :key="bonus.requiredPieces"
               class="text-xs text-gray-400 ml-2">
            • {{ bonus.requiredPieces }}件: {{ bonus.description }}
          </div>
        </div>
      </div>

      <!-- Combat Status -->
      <div v-if="playerStore.combat.inCombat" class="bg-red-900/30 border border-red-700 rounded-lg p-3">
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
