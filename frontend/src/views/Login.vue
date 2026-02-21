<template>
  <div class="min-h-screen bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center p-4">
    <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-2xl p-8 w-full max-w-md">
      <!-- Logo -->
      <div class="text-center mb-8">
        <div class="w-20 h-20 bg-indigo-100 dark:bg-indigo-900 rounded-full flex items-center justify-center mx-auto mb-4">
          <svg class="w-12 h-12 text-indigo-600 dark:text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
          </svg>
        </div>
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">School Attendance</h1>
        <p class="text-gray-600 dark:text-gray-400">{{ isLogin ? 'Sign in to your account' : 'Create your account' }}</p>
      </div>

      <!-- Error Message -->
      <div v-if="errorMessage" class="mb-4 p-3 bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-200 rounded-lg text-sm">
        {{ errorMessage }}
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit">
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Username</label>
            <input
              v-model="formData.username"
              type="text"
              required
              class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent dark:bg-gray-700 dark:text-white"
              placeholder="Enter your username"
            />
          </div>

          <div v-if="!isLogin">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Email</label>
            <input
              v-model="formData.email"
              type="email"
              required
              class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent dark:bg-gray-700 dark:text-white"
              placeholder="Enter your email"
            />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Password</label>
            <input
              v-model="formData.password"
              type="password"
              required
              class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent dark:bg-gray-700 dark:text-white"
              placeholder="Enter your password"
            />
          </div>

          <div v-if="!isLogin">
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Role</label>
            <select
              v-model="formData.role"
              class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-gray-700 dark:text-white"
            >
              <option value="Student">Student</option>
              <option value="Teacher">Teacher</option>
              <option value="Admin">Admin</option>
            </select>
          </div>
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full mt-6 px-4 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors font-semibold disabled:opacity-50"
        >
          {{ loading ? 'Please wait...' : (isLogin ? 'Sign In' : 'Sign Up') }}
        </button>
      </form>

      <!-- Toggle Login/Register -->
      <div class="mt-6 text-center">
        <button
          type="button"
          @click="isLogin = !isLogin; errorMessage = ''"
          class="text-indigo-600 dark:text-indigo-400 hover:underline text-sm"
        >
          {{ isLogin ? "Don't have an account? Sign Up" : "Already have an account? Sign In" }}
        </button>
      </div>

      <!-- Demo Credentials -->
      <div class="mt-6 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
        <p class="text-xs text-gray-600 dark:text-gray-400 font-semibold mb-2">Demo Credentials:</p>
        <div class="space-y-1 text-xs text-gray-600 dark:text-gray-400">
          <p><strong>Admin:</strong> admin / admin123</p>
          <p><strong>Teacher:</strong> teacher1 / teacher123</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '../composables/useAuth';

const router = useRouter();
const { login, register } = useAuth();

const isLogin = ref(true);
const loading = ref(false);
const errorMessage = ref('');

const formData = ref({
  username: '',
  email: '',
  password: '',
  role: 'Student',
});

async function handleSubmit() {
  loading.value = true;
  errorMessage.value = '';

  try {
    const result = isLogin.value
      ? await login(formData.value.username, formData.value.password)
      : await register(formData.value.username, formData.value.email, formData.value.password, formData.value.role);

    if (result.success) {
      router.push('/admin');
    } else {
      errorMessage.value = result.message || 'Authentication failed';
    }
  } catch (error) {
    errorMessage.value = 'An error occurred. Please try again.';
  } finally {
    loading.value = false;
  }
}
</script>
