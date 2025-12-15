<script setup lang="ts">
</script>

<template>
  <div class="bg-gray-800/80 backdrop-blur rounded-xl border border-gray-700 p-4 h-64 relative overflow-hidden">
    <h3 class="text-gray-400 text-xs uppercase tracking-widest mb-2 font-bold absolute top-4 left-4 z-10">小地圖</h3>

    <!-- Placeholder Map Visualization -->
    <div class="w-full h-full flex items-center justify-center opacity-50">
      <svg width="100%" height="100%" viewBox="0 0 200 200">
        <!-- Connections -->
        <line x1="100" y1="100" x2="100" y2="50" stroke="#4b5563" stroke-width="2" />
        <line x1="100" y1="100" x2="150" y2="100" stroke="#4b5563" stroke-width="2" />
        <line x1="100" y1="100" x2="50" y2="100" stroke="#4b5563" stroke-width="2" />

        <!-- Nodes -->
        <circle cx="100" cy="50" r="8" fill="#1f2937" stroke="#4b5563" />
        <circle cx="150" cy="100" r="8" fill="#1f2937" stroke="#4b5563" />
        <circle cx="50" cy="100" r="8" fill="#1f2937" stroke="#4b5563" />

        <!-- Current Location (Center) -->
        <circle cx="100" cy="100" r="10" fill="#3b82f6" class="animate-pulse">
          <animate attributeName="r" values="8;12;8" dur="2s" repeatCount="indefinite" />
          <animate attributeName="opacity" values="1;0.5;1" dur="2s" repeatCount="indefinite" />
        </circle>
      </svg>
    </div>

    <div class="absolute bottom-2 right-2 text-xs text-gray-500 font-mono">
      區域: 新手村
    </div>
  </div>
</template>
