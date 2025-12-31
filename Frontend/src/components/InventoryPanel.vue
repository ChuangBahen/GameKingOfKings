<script setup lang="ts">
import { ref, computed } from 'vue'
import { usePlayerStore } from '../stores/player'
import type { InventoryItemDto } from '../types/game'
import { ItemQuality, QualityColors, QualityNames } from '../types/game'

const playerStore = usePlayerStore()

// 物品類型中文對照
const typeNames: Record<string, string> = {
  weapon: '武器',
  armor: '防具',
  consumable: '消耗品',
  quest: '任務物品',
  accessory: '飾品',
  material: '材料'
}

// 取得品質顏色樣式
const getQualityStyle = (quality: number | undefined): { color: string } => {
  const q = quality ?? ItemQuality.Common
  const colorMap = QualityColors as Record<number, string>
  return { color: colorMap[q] ?? colorMap[ItemQuality.Common] ?? '#9CA3AF' }
}

// 取得品質名稱
const getQualityName = (quality: number | undefined): string => {
  const q = quality ?? ItemQuality.Common
  const nameMap = QualityNames as Record<number, string>
  return nameMap[q] ?? nameMap[ItemQuality.Common] ?? '普通'
}

// 判斷是否為裝備類型
const isEquipmentType = (type: string): boolean => {
  return ['weapon', 'armor', 'accessory'].includes(type.toLowerCase())
}

// Selected item for detail view
const selectedItem = ref<InventoryItemDto | null>(null)

// Use store's inventory data
const equippedItems = computed(() => playerStore.equippedItems)
const backpackItems = computed(() => playerStore.backpackItems)
const gold = computed(() => playerStore.inventory?.gold ?? 0)

// Active tab
const activeTab = ref<'backpack' | 'equipped'>('backpack')

// Format property display
const formatProperty = (key: string, value: number): string => {
  const propNames: Record<string, string> = {
    atk: '攻擊力',
    def: '防禦力',
    str: '力量',
    dex: '敏捷',
    int: '智力',
    wis: '智慧',
    con: '體質',
    hp: '生命值',
    mp: '魔力值'
  }
  return `${propNames[key] || key}: +${value}`
}

// Get icon based on item type
const getIcon = (item: InventoryItemDto): string => {
  if (item.icon) return item.icon
  switch (item.type) {
    case 'weapon': return '⚔️'
    case 'armor': return '🛡️'
    case 'consumable': return '🧪'
    case 'accessory': return '💍'
    case 'quest': return '📜'
    default: return '📦'
  }
}
</script>

<template>
  <div class="bg-gray-800/80 backdrop-blur rounded-xl border border-gray-700 p-4 h-full flex flex-col overflow-hidden">
    <h3 class="text-gray-400 text-xs uppercase tracking-widest mb-3 font-bold">背包</h3>

    <!-- Tabs -->
    <div class="flex gap-2 mb-3">
      <button
        @click="activeTab = 'backpack'"
        :class="[
          'px-3 py-1 rounded text-xs font-medium transition-colors',
          activeTab === 'backpack'
            ? 'bg-blue-600 text-white'
            : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
        ]"
      >
        背包 ({{ backpackItems.length }})
      </button>
      <button
        @click="activeTab = 'equipped'"
        :class="[
          'px-3 py-1 rounded text-xs font-medium transition-colors',
          activeTab === 'equipped'
            ? 'bg-green-600 text-white'
            : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
        ]"
      >
        裝備中 ({{ equippedItems.length }})
      </button>
    </div>

    <!-- Item List -->
    <div class="flex-1 overflow-y-auto space-y-1">
      <!-- Empty State -->
      <div
        v-if="(activeTab === 'backpack' && backpackItems.length === 0) || (activeTab === 'equipped' && equippedItems.length === 0)"
        class="text-gray-500 text-sm italic text-center py-4"
      >
        {{ activeTab === 'backpack' ? '背包是空的' : '沒有裝備中的物品' }}
      </div>

      <!-- Backpack Items -->
      <template v-if="activeTab === 'backpack'">
        <div
          v-for="item in backpackItems"
          :key="item.id"
          @click="selectedItem = selectedItem?.id === item.id ? null : item"
          :class="[
            'flex items-center gap-2 p-2 rounded cursor-pointer transition-all border',
            selectedItem?.id === item.id
              ? 'bg-blue-900/50 border-blue-500'
              : 'bg-gray-900/50 border-transparent hover:bg-gray-800 hover:border-gray-600'
          ]"
        >
          <span class="text-lg">{{ getIcon(item) }}</span>
          <div class="flex-1 min-w-0">
            <div
              class="text-sm truncate font-medium"
              :style="isEquipmentType(item.type) ? getQualityStyle(item.quality) : { color: 'white' }"
            >
              {{ item.name }}
            </div>
            <div class="text-xs text-gray-500 flex items-center gap-1">
              <span>{{ typeNames[item.type] || item.type }}</span>
              <span v-if="isEquipmentType(item.type) && item.quality !== undefined && item.quality > 0" class="opacity-70">
                • {{ getQualityName(item.quality) }}
              </span>
              <span v-if="item.setName" class="text-purple-400">
                [{{ item.setName }}]
              </span>
            </div>
          </div>
          <div v-if="item.quantity > 1" class="text-xs text-gray-400 bg-gray-700 px-2 py-0.5 rounded">
            x{{ item.quantity }}
          </div>
        </div>
      </template>

      <!-- Equipped Items -->
      <template v-if="activeTab === 'equipped'">
        <div
          v-for="item in equippedItems"
          :key="item.id"
          @click="selectedItem = selectedItem?.id === item.id ? null : item"
          :class="[
            'flex items-center gap-2 p-2 rounded cursor-pointer transition-all border',
            selectedItem?.id === item.id
              ? 'bg-green-900/50 border-green-500'
              : 'bg-gray-900/50 border-transparent hover:bg-gray-800 hover:border-gray-600'
          ]"
        >
          <span class="text-lg">{{ getIcon(item) }}</span>
          <div class="flex-1 min-w-0">
            <div
              class="text-sm truncate font-medium"
              :style="isEquipmentType(item.type) ? getQualityStyle(item.quality) : { color: 'white' }"
            >
              {{ item.name }}
            </div>
            <div class="text-xs text-green-400 flex items-center gap-1">
              <span>{{ item.equippedSlot || typeNames[item.type] || item.type }}</span>
              <span v-if="item.setName" class="text-purple-400">
                [{{ item.setName }}]
              </span>
            </div>
          </div>
        </div>
      </template>
    </div>

    <!-- Item Detail Panel -->
    <div
      v-if="selectedItem"
      class="mt-3 p-3 bg-gray-900/80 rounded-lg border border-gray-600"
    >
      <div class="flex items-center gap-2 mb-2">
        <span class="text-2xl">{{ getIcon(selectedItem) }}</span>
        <div>
          <div
            class="font-bold"
            :style="isEquipmentType(selectedItem.type) ? getQualityStyle(selectedItem.quality) : { color: 'white' }"
          >
            {{ selectedItem.name }}
          </div>
          <div class="text-xs text-gray-400 flex items-center gap-1">
            <span>{{ typeNames[selectedItem.type] || selectedItem.type }}</span>
            <span
              v-if="isEquipmentType(selectedItem.type) && selectedItem.quality !== undefined"
              :style="getQualityStyle(selectedItem.quality)"
            >
              • {{ getQualityName(selectedItem.quality) }}
            </span>
          </div>
        </div>
      </div>

      <!-- 套裝資訊 -->
      <div v-if="selectedItem.setName" class="text-xs text-purple-400 mb-2">
        🔗 套裝: {{ selectedItem.setName }}
      </div>

      <p v-if="selectedItem.description" class="text-xs text-gray-400 mb-2">
        {{ selectedItem.description }}
      </p>

      <div v-if="selectedItem.properties && Object.keys(selectedItem.properties).length > 0" class="space-y-1">
        <div class="text-xs text-gray-500 uppercase">屬性</div>
        <div class="grid grid-cols-2 gap-1 text-xs">
          <span
            v-for="(value, key) in selectedItem.properties"
            :key="key"
            class="text-green-400"
          >
            {{ formatProperty(String(key), value) }}
          </span>
        </div>
      </div>

      <div v-if="selectedItem.isEquipped" class="mt-2 text-xs text-green-400">
        已裝備
      </div>
    </div>

    <!-- Gold Display -->
    <div class="mt-auto pt-3 border-t border-gray-700">
      <div class="flex justify-between text-xs text-gray-400">
        <span>金幣:</span>
        <span class="text-yellow-500 font-mono">{{ gold.toLocaleString() }} 🪙</span>
      </div>
    </div>
  </div>
</template>
