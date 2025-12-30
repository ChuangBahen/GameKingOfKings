<script setup lang="ts">
import { computed } from 'vue'
import { usePlayerStore } from '../stores/player'
import { gameHub } from '../services/gameHub'

// T002: 顏色常數物件
const COLORS = {
  safe: '#4ade80',      // 綠色 - 安全出口
  danger: '#ef4444',    // 紅色 - 有怪物
  noExit: '#374151',    // 灰色 - 無出口
  player: '#3b82f6',    // 藍色 - 玩家位置
  vertical: '#a855f7'   // 紫色 - 上下方向（安全時）
} as const

// T003: 所有方向陣列
const ALL_DIRECTIONS = ['north', 'south', 'east', 'west', 'up', 'down'] as const

// T004: 垂直方向陣列
const VERTICAL_DIRECTIONS = ['up', 'down'] as const

const playerStore = usePlayerStore()

// Use store's mapData
const mapData = computed(() => playerStore.mapData)
const currentRoom = computed(() => mapData.value?.currentRoom)
const exits = computed(() => mapData.value?.exits ?? [])

// Direction positions for SVG rendering (中心點: 100, 100)
const directionPositions = {
  north: { x: 100, y: 35 },
  south: { x: 100, y: 165 },
  east: { x: 165, y: 100 },
  west: { x: 35, y: 100 },
  up: { x: 165, y: 35 },    // 右上角
  down: { x: 35, y: 165 }   // 左下角
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

// T006: 取得出口顏色（優先級：危險 > 垂直 > 安全）
const getExitColor = (dir: string): string => {
  const exitInfo = getExitInfo(dir)
  if (!exitInfo) return COLORS.noExit
  if (exitInfo.hasMonsters) return COLORS.danger
  if (isVerticalDirection(dir)) return COLORS.vertical
  return COLORS.safe
}

// T007: 判斷是否為垂直方向（上/下）
const isVerticalDirection = (dir: string): boolean => {
  return VERTICAL_DIRECTIONS.includes(dir.toLowerCase() as typeof VERTICAL_DIRECTIONS[number])
}

// T008: 取得 tooltip 文字（房間名稱 + 怪物警告）
const getTooltipText = (dir: string): string => {
  const exitInfo = getExitInfo(dir)
  if (!exitInfo) return ''
  const roomName = exitInfo.roomName || '未知'
  const monsterWarning = exitInfo.hasMonsters ? ' - 有怪物出沒' : ''
  return `${roomName}${monsterWarning}`
}

// T017: 處理出口點擊事件
const handleExitClick = (dir: string) => {
  console.log('[MiniMap] handleExitClick called:', dir, 'hasExit:', hasExit(dir))
  if (hasExit(dir)) {
    console.log('[MiniMap] Sending command: go', dir)
    gameHub.sendCommand(`go ${dir}`)
  }
}
</script>

<template>
  <div class="bg-gray-800/80 backdrop-blur rounded-xl border border-gray-700 p-4 h-full relative overflow-hidden flex flex-col">
    <h3 class="text-gray-400 text-xs uppercase tracking-widest mb-2 font-bold">小地圖</h3>

    <!-- Map Visualization -->
    <div class="flex-1 flex items-center justify-center min-h-0">
      <svg viewBox="0 0 200 200" class="w-full h-full max-w-[180px] max-h-[180px]">
        <!-- Connection lines for cardinal directions (東西南北) - 只顯示有出口的 -->
        <line
          v-for="dir in ['north', 'south', 'east', 'west'].filter(d => hasExit(d))"
          :key="dir"
          :x1="100"
          :y1="100"
          :x2="getPosition(dir).x"
          :y2="getPosition(dir).y"
          :stroke="getExitColor(dir)"
          stroke-width="2"
        />
        <!-- Connection lines for vertical directions (上/下) - 斜線虛線樣式 -->
        <line
          v-for="dir in ['up', 'down'].filter(d => hasExit(d))"
          :key="'v-' + dir"
          :x1="100"
          :y1="100"
          :x2="getPosition(dir).x"
          :y2="getPosition(dir).y"
          :stroke="getExitColor(dir)"
          stroke-width="2"
          stroke-dasharray="6,3"
        />

        <!-- T010: Exit nodes - 改為 ALL_DIRECTIONS -->
        <g v-for="dir in ALL_DIRECTIONS" :key="'node-' + dir">
          <!-- T012, T015, T018-T21: 出口節點 - 使用動態顏色和點擊事件 -->
          <circle
            v-if="hasExit(dir)"
            :cx="getPosition(dir).x"
            :cy="getPosition(dir).y"
            r="12"
            :fill="getExitInfo(dir)?.hasMonsters ? '#2d1f1f' : '#1f2937'"
            :stroke="getExitColor(dir)"
            stroke-width="2"
            class="cursor-pointer transition-all duration-150 hover:opacity-80"
            @click="handleExitClick(dir)"
          >
            <!-- T022-T024: Room name tooltip with monster warning -->
            <title>{{ getTooltipText(dir) }}</title>
          </circle>
          <!-- Direction label -->
          <text
            v-if="hasExit(dir)"
            :x="getPosition(dir).x"
            :y="getPosition(dir).y + 4"
            text-anchor="middle"
            fill="#9ca3af"
            font-size="10"
            class="pointer-events-none"
          >
            {{ directionLabels[dir] }}
          </text>
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
