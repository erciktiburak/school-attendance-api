<template>
  <div class="max-w-2xl mx-auto text-center">
    <h2 class="text-2xl font-bold text-gray-800 mb-4">Welcome to Student Portal</h2>
    <p class="text-gray-600 mb-8">Use the QR code scanner to check in when you arrive at school.</p>
    <router-link
      to="/student/scanner"
      class="inline-flex items-center px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
    >
      Open Scanner
    </router-link>
  </div>
</template>

<script setup>
</script>
