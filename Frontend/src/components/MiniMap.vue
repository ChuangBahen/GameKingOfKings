<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore } from '../stores/player'

const playerStore = usePlayerStore()

// Use store's mapData
const mapData = computed(() => playerStore.mapData)
const currentRoom = computed(() => mapData.value?.currentRoom)
const exits = computed(() => mapData.value?.exits ?? [])

// Direction positions for SVG rendering
const directionPositions = {
  north: { x: 100, y: 40 },
  south: { x: 100, y: 160 },
  east: { x: 160, y: 100 },
  west: { x: 40, y: 100 },
  up: { x: 140, y: 40 },
  down: { x: 60, y: 160 }
} as const

// Get position for a direction (with fallback)
const getPosition = (dir: string) => {
  return directionPositions[dir as keyof typeof directionPositions] ?? { x: 100, y: 100 }
}

// Direction labels in Chinese
const directionLabels: Record<string, string> = {
  north: '北',
  south: '南',
  east: '東',
  west: '西',
  up: '上',
  down: '下'
}

// Check if a direction has an exit
const hasExit = (direction: string): boolean => {
  return exits.value.some(e => e.direction.toLowerCase() === direction.toLowerCase())
}

// Get exit info for a direction
const getExitInfo = (direction: string) => {
  return exits.value.find(e => e.direction.toLowerCase() === direction.toLowerCase())
}
</script>

<template>
  <div class="bg-gray-800/80 backdrop-blur rounded-xl border border-gray-700 p-4 h-full relative overflow-hidden flex flex-col">
    <h3 class="text-gray-400 text-xs uppercase tracking-widest mb-2 font-bold">小地圖</h3>

    <!-- Map Visualization -->
    <div class="flex-1 flex items-center justify-center">
      <svg width="200" height="200" viewBox="0 0 200 200" class="opacity-80">
        <!-- Connection lines to exits -->
        <line
          v-for="dir in ['north', 'south', 'east', 'west']"
          :key="dir"
          :x1="100"
          :y1="100"
          :x2="getPosition(dir).x"
          :y2="getPosition(dir).y"
          :stroke="hasExit(dir) ? '#4ade80' : '#374151'"
          stroke-width="2"
          :stroke-dasharray="hasExit(dir) ? 'none' : '4'"
        />

        <!-- Exit nodes -->
        <g v-for="dir in ['north', 'south', 'east', 'west']" :key="'node-' + dir">
          <circle
            v-if="hasExit(dir)"
            :cx="getPosition(dir).x"
            :cy="getPosition(dir).y"
            r="12"
            fill="#1f2937"
            stroke="#4ade80"
            stroke-width="2"
            class="cursor-pointer hover:fill-gray-700"
          />
          <!-- Direction label -->
          <text
            v-if="hasExit(dir)"
            :x="getPosition(dir).x"
            :y="getPosition(dir).y + 4"
            text-anchor="middle"
            fill="#9ca3af"
            font-size="10"
          >
            {{ directionLabels[dir] }}
          </text>
          <!-- Room name tooltip -->
          <title v-if="hasExit(dir)">{{ getExitInfo(dir)?.roomName || '未知' }}</title>
        </g>

        <!-- Current Location (Center) -->
        <circle
          cx="100"
          cy="100"
          r="16"
          fill="#3b82f6"
          class="animate-pulse"
        >
          <animate attributeName="r" values="14;18;14" dur="2s" repeatCount="indefinite" />
          <animate attributeName="opacity" values="1;0.7;1" dur="2s" repeatCount="indefinite" />
        </circle>

        <!-- Player icon in center -->
        <text x="100" y="105" text-anchor="middle" fill="white" font-size="12">你</text>
      </svg>
    </div>

    <!-- Room Info -->
    <div class="mt-auto space-y-2">
      <!-- Current Room Name -->
      <div class="text-center">
        <div class="text-sm font-bold text-white">
          {{ currentRoom?.name || '未知區域' }}
        </div>
      </div>

      <!-- Monster Warning -->
      <div
        v-if="currentRoom?.monsters && currentRoom.monsters.length > 0"
        class="flex items-center justify-center gap-1 text-xs text-red-400"
      >
        <span class="animate-pulse">⚠️</span>
        <span>發現 {{ currentRoom.monsters.length }} 個怪物</span>
      </div>

      <!-- Available Exits -->
      <div class="text-xs text-gray-500 text-center">
        <span v-if="exits.length > 0">
          出口: {{ exits.map(e => directionLabels[e.direction.toLowerCase()] || e.direction).join('、') }}
        </span>
        <span v-else class="italic">無出口</span>
      </div>
    </div>
  </div>
</template>
