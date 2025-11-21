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
    errorMsg.value = 'Please enter username and password';
    return;
  }

  try {
    if (isRegistering.value) {
      await authStore.register(username.value, password.value);
    } else {
      await authStore.login(username.value, password.value);
    }
  } catch (e: any) {
    errorMsg.value = e.message || 'An error occurred';
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-900 text-white">
    <div class="w-full max-w-md p-8 space-y-6 bg-gray-800 rounded-lg shadow-xl">
      <h2 class="text-3xl font-bold text-center text-yellow-500">
        {{ isRegistering ? 'Create Account' : 'Login to King of Kings' }}
      </h2>
      
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-300">Username</label>
          <input 
            v-model="username" 
            type="text" 
            class="w-full px-4 py-2 mt-1 bg-gray-700 border border-gray-600 rounded focus:ring-2 focus:ring-yellow-500 focus:outline-none"
            placeholder="Enter username"
          />
        </div>
        
        <div>
          <label class="block text-sm font-medium text-gray-300">Password</label>
          <input 
            v-model="password" 
            type="password" 
            class="w-full px-4 py-2 mt-1 bg-gray-700 border border-gray-600 rounded focus:ring-2 focus:ring-yellow-500 focus:outline-none"
            placeholder="Enter password"
          />
        </div>

        <div v-if="errorMsg" class="text-red-500 text-sm text-center">
          {{ errorMsg }}
        </div>

        <button 
          type="submit" 
          class="w-full py-2 font-bold text-gray-900 bg-yellow-500 rounded hover:bg-yellow-400 transition-colors"
        >
          {{ isRegistering ? 'Register' : 'Login' }}
        </button>
      </form>

      <div class="text-center text-sm text-gray-400">
        <span v-if="isRegistering">Already have an account? </span>
        <span v-else>Don't have an account? </span>
        <button 
          @click="isRegistering = !isRegistering" 
          class="text-yellow-500 hover:underline"
        >
          {{ isRegistering ? 'Login here' : 'Register here' }}
        </button>
      </div>
    </div>
  </div>
</template>
