<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 rounded-2xl shadow-xl p-6">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">My Attendance History</h2>
      <p class="text-gray-600 dark:text-gray-400">View your past attendance records</p>
    </div>

    <!-- Filter -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4">
      <input
        v-model="studentNumber"
        type="text"
        placeholder="Enter your student number to view history..."
        class="w-full px-4 py-3 border-2 border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 text-center bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400"
        @change="loadHistory"
      />
    </div>

    <!-- Stats Summary -->
    <div v-if="history.length > 0" class="grid grid-cols-2 gap-4">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Days</p>
            <p class="text-2xl font-bold text-indigo-600">{{ history.length }}</p>
          </div>
          <CalendarDaysIcon class="w-8 h-8 text-indigo-600" />
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">This Month</p>
            <p class="text-2xl font-bold text-green-600">{{ thisMonthCount }}</p>
          </div>
          <ChartBarIcon class="w-8 h-8 text-green-600" />
        </div>
      </div>
    </div>

    <!-- History List -->
    <div v-if="history.length > 0" class="space-y-3">
      <div
        v-for="record in history"
        :key="record.id"
        class="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-5 hover:shadow-xl transition-shadow"
      >
        <div class="flex justify-between items-start mb-3">
          <div>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ formatDate(record.checkInTime) }}</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">{{ record.location }}</p>
          </div>
          <span :class="getStatusClass(record.status)" class="px-3 py-1 rounded-full text-xs font-semibold">
            {{ record.status }}
          </span>
        </div>

        <div class="grid grid-cols-2 gap-4 text-sm">
          <div>
            <p class="text-gray-500 dark:text-gray-400">Check-in</p>
            <p class="font-medium text-gray-900 dark:text-white">{{ formatTime(record.checkInTime) }}</p>
          </div>
          <div>
            <p class="text-gray-500 dark:text-gray-400">Check-out</p>
            <p class="font-medium text-gray-900 dark:text-white">
              {{ record.checkOutTime ? formatTime(record.checkOutTime) : '-' }}
            </p>
          </div>
        </div>

        <div v-if="record.checkOutTime" class="mt-3 pt-3 border-t dark:border-gray-700">
          <div class="flex items-center text-sm text-gray-600 dark:text-gray-400">
            <ClockIcon class="w-4 h-4 mr-1" />
            <span>Duration: {{ calculateDuration(record) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="bg-white dark:bg-gray-800 rounded-2xl shadow-xl p-12 text-center">
      <ClipboardDocumentListIcon class="w-16 h-16 text-gray-400 dark:text-gray-500 mx-auto mb-4" />
      <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">No History Found</h3>
      <p class="text-gray-600 dark:text-gray-400">
        {{ studentNumber ? 'No attendance records found for this student.' : 'Enter your student number above to view your history.' }}
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import {
  CalendarDaysIcon,
  ChartBarIcon,
  ClockIcon,
  ClipboardDocumentListIcon,
} from '@heroicons/vue/24/outline';
import api from '../../services/api';

const studentNumber = ref('');
const history = ref([]);

const thisMonthCount = computed(() => {
  const now = new Date();
  const thisMonth = now.getMonth();
  const thisYear = now.getFullYear();
  
  return history.value.filter(record => {
    const date = new Date(record.checkInTime);
    return date.getMonth() === thisMonth && date.getFullYear() === thisYear;
  }).length;
});

async function loadHistory() {
  if (!studentNumber.value) return;
  
  try {
    // Get student by number
    const studentsResponse = await api.getStudents();
    const student = studentsResponse.data.find(s => s.studentNumber === studentNumber.value);
    
    if (!student) {
      history.value = [];
      return;
    }

    const response = await api.getStudentAttendance(student.id);
    history.value = response.data;
  } catch (error) {
    console.error('Error loading history:', error);
    history.value = [];
  }
}

function formatDate(datetime) {
  return new Date(datetime).toLocaleDateString('en-US', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  });
}

function formatTime(datetime) {
  return new Date(datetime).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  });
}

function calculateDuration(record) {
  if (!record.checkOutTime) return '-';
  
  const checkIn = new Date(record.checkInTime);
  const checkOut = new Date(record.checkOutTime);
  const minutes = Math.round((checkOut - checkIn) / 60000);
  
  if (minutes < 60) return `${minutes} minutes`;
  
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  return `${hours}h ${remainingMinutes}m`;
}

function getStatusClass(status) {
  const classes = {
    Present: 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-400',
    Late: 'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-400',
    Absent: 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-400',
    Excused: 'bg-purple-100 dark:bg-purple-900/30 text-purple-800 dark:text-purple-400',
  };
  return classes[status] || 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300';
}
</script>