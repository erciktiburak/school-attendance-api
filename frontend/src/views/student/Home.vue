<template>
  <div class="space-y-6">
    <!-- Welcome Card -->
    <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-xl p-8 text-center">
      <div class="w-24 h-24 bg-indigo-100 dark:bg-indigo-900 rounded-full flex items-center justify-center text-indigo-600 dark:text-indigo-400 text-3xl font-bold mx-auto mb-4">
        {{ initials }}
      </div>
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">Welcome Back!</h2>
      <p class="text-gray-600 dark:text-gray-400 mb-1">{{ studentInfo?.firstName }} {{ studentInfo?.lastName }}</p>
      <p class="text-sm text-gray-500 dark:text-gray-400">{{ studentInfo?.studentNumber }}</p>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
      <router-link
        to="/student/scanner"
        class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-6 hover:shadow-2xl transition-shadow"
      >
        <div class="flex items-center">
          <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-full">
            <QrCodeIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Scan QR Code</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Check-in or check-out</p>
          </div>
        </div>
      </router-link>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-6">
        <div class="flex items-center">
          <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-full">
            <ChartBarIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ stats?.attendanceRate || 0 }}%</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">Attendance Rate</p>
          </div>
        </div>
      </div>
    </div>

    <!-- My QR Code -->
    <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-xl p-8">
      <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-4 text-center">My QR Code</h3>
      <div class="bg-gray-50 dark:bg-gray-700 p-6 rounded-xl mb-4">
        <img
          v-if="studentInfo"
          :src="api.getStudentQRCode(studentInfo.id)"
          alt="My QR Code"
          class="mx-auto w-64 h-64"
        />
      </div>
      <p class="text-center text-sm text-gray-600 dark:text-gray-400">Show this QR code at the entrance</p>
      <button
        @click="downloadQR"
        class="w-full mt-4 px-4 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors font-medium"
      >
        Download QR Code
      </button>
    </div>

    <!-- Stats Summary -->
    <div class="grid grid-cols-3 gap-4">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4 text-center">
        <p class="text-2xl font-bold text-indigo-600 dark:text-indigo-400">{{ stats?.totalAttendance || 0 }}</p>
        <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Total Days</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4 text-center">
        <p class="text-2xl font-bold text-green-600 dark:text-green-400">{{ stats?.presentCount || 0 }}</p>
        <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Present</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4 text-center">
        <p class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ stats?.lateCount || 0 }}</p>
        <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Late</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { QrCodeIcon, ChartBarIcon } from '@heroicons/vue/24/outline';
import api from '../../services/api';

const studentInfo = ref(null);
const stats = ref(null);

const initials = computed(() => {
  if (!studentInfo.value) return '??';
  return `${studentInfo.value.firstName.charAt(0)}${studentInfo.value.lastName.charAt(0)}`;
});

async function loadStudentData() {
  try {
    // For demo purposes, load first student
    // In real app, this would be based on logged-in user
    const studentsResponse = await api.getStudents();
    studentInfo.value = studentsResponse.data[0];
    
    if (studentInfo.value) {
      const statsResponse = await api.getStudentStats(studentInfo.value.id);
      stats.value = statsResponse.data;
    }
  } catch (error) {
    console.error('Error loading student data:', error);
  }
}

function downloadQR() {
  if (!studentInfo.value) return;
  
  const link = document.createElement('a');
  link.href = api.getStudentQRCode(studentInfo.value.id);
  link.download = `${studentInfo.value.studentNumber}_qrcode.png`;
  link.click();
}

onMounted(() => {
  loadStudentData();
});
</script>