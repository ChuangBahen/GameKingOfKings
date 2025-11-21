import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '');
  const username = ref(localStorage.getItem('username') || '');

  const isAuthenticated = computed(() => !!token.value);

  function setAuth(newToken: string, newUsername: string) {
    token.value = newToken;
    username.value = newUsername;
    localStorage.setItem('token', newToken);
    localStorage.setItem('username', newUsername);
  }

  function logout() {
    token.value = '';
    username.value = '';
    localStorage.removeItem('token');
    localStorage.removeItem('username');
  }

  async function login(usernameInput: string, passwordInput: string) {
    try {
      const response = await fetch('http://localhost:5000/api/Auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: usernameInput, password: passwordInput }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Login failed');
      }

      const data = await response.json();
      setAuth(data.token, data.username);
      return true;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  async function register(usernameInput: string, passwordInput: string) {
    try {
      const response = await fetch('http://localhost:5000/api/Auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username: usernameInput, password: passwordInput }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Registration failed');
      }

      const data = await response.json();
      setAuth(data.token, data.username);
      return true;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  return {
    token,
    username,
    isAuthenticated,
    login,
    logout,
    register,
  };
});
