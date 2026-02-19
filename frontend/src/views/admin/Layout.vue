<template>
    <div class="flex h-screen bg-gray-100">
      <!-- Sidebar -->
      <aside class="w-64 bg-white shadow-lg">
        <div class="p-6">
          <h1 class="text-2xl font-bold text-indigo-600">School Attendance</h1>
          <p class="text-sm text-gray-500">Admin Panel</p>
        </div>
        
        <nav class="mt-6">
          <router-link
            v-for="item in menuItems"
            :key="item.name"
            :to="item.path"
            class="flex items-center px-6 py-3 text-gray-700 hover:bg-indigo-50 hover:text-indigo-600 transition-colors"
            active-class="bg-indigo-50 text-indigo-600 border-r-4 border-indigo-600"
          >
            <component :is="item.icon" class="w-5 h-5 mr-3" />
            <span class="font-medium">{{ item.name }}</span>
          </router-link>
        </nav>
  
        <!-- Student Portal Link -->
        <div class="absolute bottom-0 w-64 p-6 border-t">
          <router-link
            to="/student"
            class="flex items-center justify-center px-4 py-2 text-sm text-indigo-600 border border-indigo-600 rounded-lg hover:bg-indigo-600 hover:text-white transition-colors"
          >
            Switch to Student Portal
          </router-link>
        </div>
      </aside>
  
      <!-- Main Content -->
      <div class="flex-1 flex flex-col overflow-hidden">
        <!-- Top Navbar -->
        <header class="bg-white shadow-sm">
          <div class="flex items-center justify-between px-8 py-4">
            <h2 class="text-xl font-semibold text-gray-800">{{ currentPageTitle }}</h2>
            <div class="flex items-center space-x-4">
              <span class="text-sm text-gray-600">{{ currentDate }}</span>
              <div class="w-10 h-10 bg-indigo-600 rounded-full flex items-center justify-center text-white font-semibold">
                A
              </div>
            </div>
          </div>
        </header>
  
        <!-- Page Content -->
        <main class="flex-1 overflow-y-auto p-8">
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
    UsersIcon,
    ClipboardDocumentListIcon,
    ChartBarIcon,
  } from '@heroicons/vue/24/outline';
  
  const route = useRoute();
  
  const menuItems = [
    { name: 'Dashboard', path: '/admin', icon: HomeIcon },
    { name: 'Students', path: '/admin/students', icon: UsersIcon },
    { name: 'Attendance', path: '/admin/attendance', icon: ClipboardDocumentListIcon },
    { name: 'Reports', path: '/admin/reports', icon: ChartBarIcon },
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
  </script>