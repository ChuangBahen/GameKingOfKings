<script setup lang="ts">
import { ref, computed } from 'vue'
import { usePlayerStore } from '../stores/player'
import type { SkillDto } from '../types/game'

const playerStore = usePlayerStore()

// Use store's skills data
const learnedSkills = computed(() => playerStore.learnedSkills)
const lockedSkills = computed(() => playerStore.lockedSkills)

// Active tab
const activeTab = ref<'learned' | 'locked'>('learned')

// Selected skill for detail view
const selectedSkill = ref<SkillDto | null>(null)

// Skill type icons
const getSkillIcon = (type: string): string => {
  switch (type?.toLowerCase()) {
    case 'attack': return '⚔️'
    case 'magic': return '✨'
    case 'heal': return '💚'
    case 'buff': return '🔺'
    case 'debuff': return '🔻'
    case 'passive': return '🛡️'
    default: return '📜'
  }
}

// Skill type names in Chinese
const skillTypeNames: Record<string, string> = {
  attack: '攻擊',
  magic: '魔法',
  heal: '治療',
  buff: '增益',
  debuff: '減益',
  passive: '被動'
}
</script>

<template>
  <div class="bg-gray-800/90 backdrop-blur p-4 rounded-xl border border-gray-700 text-white h-full flex flex-col overflow-hidden">
    <h2 class="text-xl font-bold mb-4 text-purple-400 border-b border-gray-600 pb-2">
      ⚡ 技能
    </h2>

    <!-- Tabs -->
    <div class="flex gap-2 mb-3">
      <button
        @click="activeTab = 'learned'"
        :class="[
          'px-3 py-1 rounded text-xs font-medium transition-colors',
          activeTab === 'learned'
            ? 'bg-purple-600 text-white'
            : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
        ]"
      >
        已學習 ({{ learnedSkills.length }})
      </button>
      <button
        @click="activeTab = 'locked'"
        :class="[
          'px-3 py-1 rounded text-xs font-medium transition-colors',
          activeTab === 'locked'
            ? 'bg-gray-600 text-white'
            : 'bg-gray-700 text-gray-400 hover:bg-gray-600'
        ]"
      >
        未解鎖 ({{ lockedSkills.length }})
      </button>
    </div>

    <!-- Skill List -->
    <div class="flex-1 overflow-y-auto space-y-1">
      <!-- Empty State -->
      <div
        v-if="(activeTab === 'learned' && learnedSkills.length === 0) || (activeTab === 'locked' && lockedSkills.length === 0)"
        class="text-gray-500 text-sm italic text-center py-4"
      >
        {{ activeTab === 'learned' ? '尚未學習任何技能' : '沒有更多可學習的技能' }}
      </div>

      <!-- Learned Skills -->
      <template v-if="activeTab === 'learned'">
        <div
          v-for="skill in learnedSkills"
          :key="skill.skillId"
          @click="selectedSkill = selectedSkill?.skillId === skill.skillId ? null : skill"
          :class="[
            'flex items-center gap-2 p-2 rounded cursor-pointer transition-all border',
            selectedSkill?.skillId === skill.skillId
              ? 'bg-purple-900/50 border-purple-500'
              : 'bg-gray-900/50 border-transparent hover:bg-gray-800 hover:border-gray-600'
          ]"
        >
          <span class="text-lg">{{ getSkillIcon(skill.type) }}</span>
          <div class="flex-1 min-w-0">
            <div class="text-sm text-white truncate">{{ skill.name }}</div>
            <div class="text-xs text-purple-400">{{ skillTypeNames[skill.type?.toLowerCase()] || skill.type }}</div>
          </div>
          <div class="text-xs text-blue-400 bg-blue-900/30 px-2 py-0.5 rounded">
            MP {{ skill.mpCost }}
          </div>
        </div>
      </template>

      <!-- Locked Skills -->
      <template v-if="activeTab === 'locked'">
        <div
          v-for="skill in lockedSkills"
          :key="skill.skillId"
          @click="selectedSkill = selectedSkill?.skillId === skill.skillId ? null : skill"
          :class="[
            'flex items-center gap-2 p-2 rounded cursor-pointer transition-all border opacity-60',
            selectedSkill?.skillId === skill.skillId
              ? 'bg-gray-700/50 border-gray-500'
              : 'bg-gray-900/50 border-transparent hover:bg-gray-800 hover:border-gray-600'
          ]"
        >
          <span class="text-lg grayscale">{{ getSkillIcon(skill.type) }}</span>
          <div class="flex-1 min-w-0">
            <div class="text-sm text-gray-400 truncate">{{ skill.name }}</div>
            <div class="text-xs text-gray-500">需要等級 {{ skill.requiredLevel }}</div>
          </div>
          <div class="text-xs text-gray-500 bg-gray-700 px-2 py-0.5 rounded">
            🔒
          </div>
        </div>
      </template>
    </div>

    <!-- Skill Detail Panel -->
    <div
      v-if="selectedSkill"
      class="mt-3 p-3 bg-gray-900/80 rounded-lg border border-gray-600"
    >
      <div class="flex items-center gap-2 mb-2">
        <span class="text-2xl" :class="{ 'grayscale': !selectedSkill.isLearned }">
          {{ getSkillIcon(selectedSkill.type) }}
        </span>
        <div>
          <div class="text-white font-bold">{{ selectedSkill.name }}</div>
          <div class="text-xs text-gray-400">
            {{ skillTypeNames[selectedSkill.type?.toLowerCase()] || selectedSkill.type }}
          </div>
        </div>
      </div>

      <p v-if="selectedSkill.description" class="text-xs text-gray-400 mb-2">
        {{ selectedSkill.description }}
      </p>

      <div class="grid grid-cols-2 gap-2 text-xs">
        <div class="flex justify-between">
          <span class="text-gray-500">MP 消耗</span>
          <span class="text-blue-400">{{ selectedSkill.mpCost }}</span>
        </div>
        <div class="flex justify-between">
          <span class="text-gray-500">需求等級</span>
          <span :class="selectedSkill.isLearned ? 'text-green-400' : 'text-yellow-400'">
            {{ selectedSkill.requiredLevel }}
          </span>
        </div>
      </div>

      <div class="mt-2 pt-2 border-t border-gray-700">
        <div v-if="selectedSkill.isLearned" class="text-xs text-green-400">
          ✓ 已學習 - 使用指令: skill {{ selectedSkill.skillId }}
        </div>
        <div v-else class="text-xs text-yellow-400">
          🔒 需要達到等級 {{ selectedSkill.requiredLevel }} 才能學習
        </div>
      </div>
    </div>
  </div>
</template>
