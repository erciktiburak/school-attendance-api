<template>
  <div class="min-h-screen bg-gradient-to-br from-indigo-500 to-purple-600">
    <!-- Top Navigation -->
    <nav class="bg-white shadow-lg">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <div class="flex items-center">
            <h1 class="text-2xl font-bold text-indigo-600">Student Portal</h1>
          </div>
          <router-link
            to="/admin"
            class="text-sm text-gray-600 hover:text-indigo-600"
          >
            Admin Panel →
          </router-link>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <router-view />
    </div>

    <!-- Bottom Navigation -->
    <div class="fixed bottom-0 left-0 right-0 bg-white shadow-lg border-t">
      <div class="max-w-7xl mx-auto px-4">
        <div class="flex justify-around py-3">
          <router-link
            v-for="item in menuItems"
            :key="item.path"
            :to="item.path"
            class="flex flex-col items-center px-4 py-2 text-gray-600 hover:text-indigo-600 transition-colors"
            active-class="text-indigo-600"
          >
            <component :is="item.icon" class="w-6 h-6 mb-1" />
            <span class="text-xs font-medium">{{ item.name }}</span>
          </router-link>
        </div>
      </div>
    </div>

    <!-- Spacing for bottom nav -->
    <div class="h-20"></div>
  </div>
</template>

<script setup>
import { HomeIcon, QrCodeIcon, ClockIcon } from '@heroicons/vue/24/outline';

const menuItems = [
  { name: 'Home', path: '/student', icon: HomeIcon },
  { name: 'Scanner', path: '/student/scanner', icon: QrCodeIcon },
  { name: 'History', path: '/student/history', icon: ClockIcon },
];
</script>