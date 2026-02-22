<template>
  <div class="flex h-screen bg-gray-100 dark:bg-gray-900">
    <!-- Sidebar -->
    <aside class="relative w-64 bg-white dark:bg-gray-800 shadow-lg">
      <div class="p-6">
        <h1 class="text-2xl font-bold text-indigo-600 dark:text-indigo-400">School Attendance</h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">Admin Panel</p>
      </div>

      <nav class="mt-6">
        <router-link
          v-for="item in menuItems"
          :key="item.name"
          :to="item.path"
          class="flex items-center px-6 py-3 text-gray-700 dark:text-gray-300 hover:bg-indigo-50 dark:hover:bg-gray-700 hover:text-indigo-600 dark:hover:text-indigo-400 transition-colors"
          active-class="bg-indigo-50 dark:bg-gray-700 text-indigo-600 dark:text-indigo-400 border-r-4 border-indigo-600"
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

      <!-- Student Portal Link -->
      <div class="absolute bottom-0 w-64 p-6 border-t dark:border-gray-700">
        <router-link
          to="/student"
          class="flex items-center justify-center px-4 py-2 text-sm text-indigo-600 dark:text-indigo-400 border border-indigo-600 dark:border-indigo-400 rounded-lg hover:bg-indigo-600 hover:text-white dark:hover:bg-indigo-500 transition-colors"
        >
          Switch to Student Portal
        </router-link>
      </div>
    </aside>

    <!-- Main Content -->
    <div class="flex-1 flex flex-col overflow-hidden">
      <!-- Top Navbar -->
      <header class="bg-white dark:bg-gray-800 shadow-sm">
        <div class="flex items-center justify-between px-8 py-4">
          <h2 class="text-xl font-semibold text-gray-800 dark:text-gray-200">{{ currentPageTitle }}</h2>
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-600 dark:text-gray-400">{{ currentDate }}</span>
            <span class="text-sm text-gray-600 dark:text-gray-400">{{ user?.username || 'Guest' }}</span>
            <button
              @click="handleLogout"
              class="text-sm text-red-600 dark:text-red-400 hover:underline"
            >
              Logout
            </button>
            <div class="w-10 h-10 bg-indigo-600 dark:bg-indigo-500 rounded-full flex items-center justify-center text-white font-semibold">
              {{ user?.username?.charAt(0).toUpperCase() || 'A' }}
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="flex-1 overflow-y-auto p-8 bg-gray-100 dark:bg-gray-900">
        <router-view />
      </main>
    </div>

    <NotificationToast />
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted } from 'vue';
import { useRoute } from 'vue-router';
import NotificationToast from '../../components/NotificationToast.vue';
import { useSignalR } from '../../composables/useSignalR';
import {
  HomeIcon,
  UsersIcon,
  ClipboardDocumentListIcon,
  ChartBarIcon,
  AcademicCapIcon,
  MoonIcon,
  SunIcon,
} from '@heroicons/vue/24/outline';
import { useDarkMode } from '../../composables/useDarkMode';
import { useAuth } from '../../composables/useAuth';

const route = useRoute();
const { isDark, toggle: toggleDarkMode } = useDarkMode();
const { user, logout } = useAuth();
const { connect, disconnect } = useSignalR();

onMounted(() => connect());
onUnmounted(() => disconnect());

const menuItems = [
  { name: 'Dashboard', path: '/admin', icon: HomeIcon },
  { name: 'Students', path: '/admin/students', icon: UsersIcon },
  { name: 'Attendance', path: '/admin/attendance', icon: ClipboardDocumentListIcon },
  { name: 'Reports', path: '/admin/reports', icon: ChartBarIcon },
  { name: 'Courses', path: '/admin/courses', icon: AcademicCapIcon },
];

const currentPageTitle = computed(() => {
  return route.name || 'Dashboard';
});

const currentDate = computed(() => {
  return new Date().toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
});

function handleLogout() {
  logout();
}
</script>
