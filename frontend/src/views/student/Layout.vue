<template>
  <div class="flex flex-col min-h-screen bg-gray-100">
    <header class="bg-white shadow-sm">
      <div class="flex items-center justify-between px-6 py-4">
        <h1 class="text-xl font-bold text-indigo-600">Student Portal</h1>
        <nav class="flex gap-4">
          <router-link
            to="/student"
            class="text-gray-600 hover:text-indigo-600"
            active-class="text-indigo-600 font-medium"
          >
            Home
          </router-link>
          <router-link
            to="/student/scanner"
            class="text-gray-600 hover:text-indigo-600"
            active-class="text-indigo-600 font-medium"
          >
            Check-in
          </router-link>
          <router-link
            to="/student/history"
            class="text-gray-600 hover:text-indigo-600"
            active-class="text-indigo-600 font-medium"
          >
            History
          </router-link>
          <router-link
            to="/admin"
            class="text-sm text-indigo-600 border border-indigo-600 px-3 py-1 rounded hover:bg-indigo-600 hover:text-white"
          >
            Admin
          </router-link>
        </nav>
      </div>
    </header>

    <main class="flex-1 p-6">
      <router-view />
    </main>
  </div>
</template>

<script setup>
</script>
