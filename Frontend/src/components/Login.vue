<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();

const username = ref('');
const password = ref('');
const isRegistering = ref(false);
const errorMsg = ref('');

async function handleSubmit() {
  errorMsg.value = '';
  if (!username.value || !password.value) {
    errorMsg.value = '請輸入帳號和密碼';
    return;
  }

  try {
    if (isRegistering.value) {
      await authStore.register(username.value, password.value);
    } else {
      await authStore.login(username.value, password.value);
    }
  } catch (e: any) {
    errorMsg.value = e.message || '發生錯誤，請重試';
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-900 text-white">
    <div class="w-full max-w-md p-8 space-y-6 bg-gray-800 rounded-lg shadow-xl">
      <h2 class="text-3xl font-bold text-center text-yellow-500">
        {{ isRegistering ? '建立帳號' : '登入萬王之王' }}
      </h2>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-300">帳號</label>
          <input
            v-model="username"
            type="text"
            class="w-full px-4 py-2 mt-1 bg-gray-700 border border-gray-600 rounded focus:ring-2 focus:ring-yellow-500 focus:outline-none"
            placeholder="輸入帳號"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-300">密碼</label>
          <input
            v-model="password"
            type="password"
            class="w-full px-4 py-2 mt-1 bg-gray-700 border border-gray-600 rounded focus:ring-2 focus:ring-yellow-500 focus:outline-none"
            placeholder="輸入密碼"
          />
        </div>

        <div v-if="errorMsg" class="text-red-500 text-sm text-center">
          {{ errorMsg }}
        </div>

        <button
          type="submit"
          class="w-full py-2 font-bold text-gray-900 bg-yellow-500 rounded hover:bg-yellow-400 transition-colors"
        >
          {{ isRegistering ? '註冊' : '登入' }}
        </button>
      </form>

      <div class="text-center text-sm text-gray-400">
        <span v-if="isRegistering">已有帳號？</span>
        <span v-else>還沒有帳號？</span>
        <button
          @click="isRegistering = !isRegistering"
          class="text-yellow-500 hover:underline"
        >
          {{ isRegistering ? '前往登入' : '立即註冊' }}
        </button>
      </div>
    </div>
  </div>
</template>
