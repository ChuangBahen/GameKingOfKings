<template>
  <div class="game-container">
    
    <!-- Login Overlay -->
    <div v-if="!hasJoined" class="login-overlay">
        <div class="login-box">
            <h2 class="login-title">Enter World</h2>
            <input 
                v-model="username" 
                @keyup.enter="joinGame"
                type="text" 
                class="login-input"
                placeholder="Username"
                autofocus
            />
            <button @click="joinGame" class="login-button">
                Join Game
            </button>
        </div>
    </div>

    <!-- Game Output -->
    <div class="game-output" ref="outputContainer">
        <div v-for="(msg, index) in messages" :key="index" v-html="msg" class="message"></div>
    </div>

    <!-- Input Area -->
    <div class="input-area">
        <input 
            v-model="userInput" 
            @keyup.enter="sendCommand"
            type="text" 
            class="command-input"
            placeholder="Enter command..."
        />
        <button @click="sendCommand" class="send-button">
            Send
        </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, nextTick } from 'vue'
import { signalRService } from '../services/signalr'

const messages = signalRService.messages
const userInput = ref('')
const username = ref('')
const hasJoined = ref(false)
const outputContainer = ref<HTMLElement | null>(null)

onMounted(async () => {
    await signalRService.start()
})

// Auto-scroll to bottom
watch(messages.value, async () => {
    await nextTick()
    if (outputContainer.value) {
        outputContainer.value.scrollTop = outputContainer.value.scrollHeight
    }
})

const joinGame = async () => {
    if (!username.value) return
    await signalRService.joinGame(username.value)
    hasJoined.value = true
}

const sendCommand = async () => {
    if (!userInput.value) return
    await signalRService.sendCommand(userInput.value)
    userInput.value = ''
}
</script>

<style scoped>
.game-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #1a1b26;
  color: #a9b1d6;
  font-family: 'Courier New', monospace;
  padding: 1rem;
}

.login-overlay {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}

.login-box {
  background-color: #16161e;
  padding: 2rem;
  border-radius: 0.5rem;
  border: 1px solid #7aa2f7;
  width: 24rem;
  max-width: 90%;
}

.login-title {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  color: #7aa2f7;
  text-align: center;
}

.login-input {
  width: 100%;
  background-color: #1a1b26;
  border: 1px solid #565f89;
  border-radius: 0.25rem;
  padding: 0.5rem 1rem;
  margin-bottom: 1rem;
  color: #a9b1d6;
  font-family: inherit;
}

.login-input:focus {
  outline: none;
  border-color: #7aa2f7;
}

.login-button {
  width: 100%;
  background-color: #7aa2f7;
  color: #1a1b26;
  font-weight: bold;
  padding: 0.5rem;
  border-radius: 0.25rem;
  border: none;
  cursor: pointer;
  font-family: inherit;
}

.login-button:hover {
  background-color: #2ac3de;
}

.game-output {
  flex: 1;
  overflow-y: auto;
  border: 1px solid #7aa2f7;
  padding: 1rem;
  border-radius: 0.25rem;
  margin-bottom: 1rem;
  background-color: rgba(0, 0, 0, 0.3);
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.2);
}

.message {
  margin-bottom: 0.25rem;
}

.input-area {
  display: flex;
  gap: 0.5rem;
}

.command-input {
  flex: 1;
  background-color: #16161e;
  border: 1px solid #7aa2f7;
  border-radius: 0.25rem;
  padding: 0.5rem 1rem;
  color: #a9b1d6;
  font-family: inherit;
}

.command-input:focus {
  outline: none;
  box-shadow: 0 0 0 2px #7aa2f7;
}

.send-button {
  background-color: #7aa2f7;
  color: #1a1b26;
  font-weight: bold;
  padding: 0.5rem 1.5rem;
  border-radius: 0.25rem;
  border: none;
  cursor: pointer;
  transition: background-color 0.2s;
  font-family: inherit;
}

.send-button:hover {
  background-color: #2ac3de;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}
::-webkit-scrollbar-track {
  background: #1a1b26; 
}
::-webkit-scrollbar-thumb {
  background: #565f89; 
  border-radius: 4px;
}
::-webkit-scrollbar-thumb:hover {
  background: #7aa2f7; 
}
</style>
