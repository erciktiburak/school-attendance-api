<template>
  <div class="flex h-screen bg-gray-100 dark:bg-gray-900">
    <!-- Sidebar -->
    <aside class="relative w-64 bg-white dark:bg-gray-800 shadow-lg">
      <div class="p-6">
        <h1 class="text-2xl font-bold text-purple-600 dark:text-purple-400">Teacher Portal</h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">Attendance Management</p>
      </div>

      <nav class="mt-6">
        <router-link
          v-for="item in menuItems"
          :key="item.name"
          :to="item.path"
          class="flex items-center px-6 py-3 text-gray-700 dark:text-gray-300 hover:bg-purple-50 dark:hover:bg-gray-700 hover:text-purple-600 dark:hover:text-purple-400 transition-colors"
          active-class="bg-purple-50 dark:bg-gray-700 text-purple-600 dark:text-purple-400 border-r-4 border-purple-600"
        >
          <component :is="item.icon" class="w-5 h-5 mr-3" />
          <span class="font-medium">{{ item.name }}</span>
        </router-link>
      </nav>

      <!-- Dark Mode Toggle -->
      <div class="absolute bottom-20 w-64 px-6">
        <button
          @click="toggleDarkMode"
          class="w-full flex items-center justify-center px-4 py-2 text-sm bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
        >
          <component :is="isDark ? SunIcon : MoonIcon" class="w-5 h-5 mr-2" />
          {{ isDark ? 'Light Mode' : 'Dark Mode' }}
        </button>
      </div>

      <!-- Logout -->
      <div class="absolute bottom-0 w-64 p-6 border-t dark:border-gray-700">
        <button
          @click="handleLogout"
          class="w-full px-4 py-2 text-sm text-red-600 dark:text-red-400 border border-red-600 dark:border-red-400 rounded-lg hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors"
        >
          Logout
        </button>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <!-- Top Navbar -->
      <header class="bg-white dark:bg-gray-800 shadow-sm">
        <div class="flex items-center justify-between px-8 py-4">
          <h2 class="text-xl font-semibold text-gray-800 dark:text-gray-200">{{ currentPageTitle }}</h2>
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{ user?.username || 'Teacher' }}</span>
            <div class="w-10 h-10 bg-purple-600 dark:bg-purple-500 rounded-full flex items-center justify-center text-white font-semibold">
              {{ user?.username?.charAt(0).toUpperCase() || 'T' }}
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 overflow-y-auto p-8 bg-gray-100 dark:bg-gray-900">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import {
  HomeIcon,
  ExclamationTriangleIcon,
  DocumentTextIcon,
  MoonIcon,
  SunIcon,
} from '@heroicons/vue/24/outline';
import { useDarkMode } from '../../composables/useDarkMode';
import { useAuth } from '../../composables/useAuth';

const route = useRoute();
const { isDark, toggle: toggleDarkMode } = useDarkMode();
const { user, logout } = useAuth();

const menuItems = [
  { name: 'Dashboard', path: '/teacher', icon: HomeIcon },
  { name: 'At-Risk Students', path: '/teacher/at-risk', icon: ExclamationTriangleIcon },
  { name: 'Daily Report', path: '/teacher/daily-report', icon: DocumentTextIcon },
];

const currentPageTitle = computed(() => {
  const name = route.name || '';
  return name === 'TeacherDashboard' ? 'Dashboard' : name.replace(/([A-Z])/g, ' $1').trim() || 'Dashboard';
});

function handleLogout() {
  logout();
}
</script>
