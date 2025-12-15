<script setup lang="ts">
import { ref } from 'vue'

// 物品類型中文對照
const typeNames: Record<string, string> = {
  weapon: '武器',
  armor: '防具',
  consumable: '消耗品',
  quest: '任務物品'
}

const inventory = ref(Array(25).fill(null).map((_, i) => {
  if (i === 0) return { name: '生鏽的劍', icon: '🗡️', type: 'weapon' }
  if (i === 1) return { name: '生命藥水', icon: '🧪', type: 'consumable' }
  return null
}))
</script>

<template>
  <div class="bg-gray-800/80 backdrop-blur rounded-xl border border-gray-700 p-4 h-full flex flex-col">
    <h3 class="text-gray-400 text-xs uppercase tracking-widest mb-3 font-bold">背包</h3>

    <div class="grid grid-cols-5 gap-2">
      <div v-for="(item, index) in inventory" :key="index"
           class="aspect-square bg-gray-900/50 rounded border border-gray-700 flex items-center justify-center relative group cursor-pointer hover:border-blue-500 hover:bg-gray-800 transition-all">

        <span v-if="item" class="text-2xl filter drop-shadow-md">{{ item.icon }}</span>

        <!-- Tooltip -->
        <div v-if="item" class="absolute bottom-full mb-2 left-1/2 transform -translate-x-1/2 w-32 bg-black/90 border border-gray-600 rounded p-2 text-xs z-20 opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none">
          <div class="font-bold text-white">{{ item.name }}</div>
          <div class="text-gray-400">{{ typeNames[item.type] || item.type }}</div>
        </div>
      </div>
    </div>

    <div class="mt-auto pt-4 border-t border-gray-700">
      <div class="flex justify-between text-xs text-gray-400">
        <span>金幣:</span>
        <span class="text-yellow-500 font-mono">1,250 🪙</span>
      </div>
    </div>
  </div>
</template>
