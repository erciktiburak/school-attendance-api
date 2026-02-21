<template>
  <div>
    <div class="mb-6 flex flex-wrap items-center gap-4">
      <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white">Daily Report</h2>
        <p class="text-gray-600 dark:text-gray-400">Attendance summary by date</p>
      </div>
      <input
        v-model="selectedDate"
        type="date"
        @change="loadSummary"
        class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-purple-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
      />
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-2 md:grid-cols-5 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">Total Students</p>
        <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ summary?.totalStudents || 0 }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">Present</p>
        <p class="text-2xl font-bold text-green-600 dark:text-green-400">{{ summary?.presentCount || 0 }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">Late</p>
        <p class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ summary?.lateCount || 0 }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">Absent</p>
        <p class="text-2xl font-bold text-red-600 dark:text-red-400">{{ summary?.absentCount || 0 }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">Attendance Rate</p>
        <p class="text-2xl font-bold text-purple-600 dark:text-purple-400">{{ summary?.attendanceRate ?? 0 }}%</p>
      </div>
    </div>

    <!-- Recent Check-ins -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <div class="p-6 border-b dark:border-gray-700">
        <h3 class="text-lg font-semibold dark:text-white">Recent Check-ins</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Student</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Time</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Status</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="r in summary?.recentCheckIns" :key="r.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-full flex items-center justify-center text-purple-600 dark:text-purple-400 font-semibold mr-3">
                    {{ r.student?.firstName?.charAt(0) }}{{ r.student?.lastName?.charAt(0) }}
                  </div>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">{{ r.student?.firstName }} {{ r.student?.lastName }}</p>
                    <p class="text-sm text-gray-500 dark:text-gray-400">{{ r.student?.studentNumber }}</p>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-400">{{ formatTime(r.checkInTime) }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-400': r.status === 'Present',
                    'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-400': r.status === 'Late',
                    'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-400': r.status === 'Absent',
                    'bg-purple-100 dark:bg-purple-900/30 text-purple-800 dark:text-purple-400': r.status === 'Excused',
                  }"
                  class="px-2 py-1 text-xs font-semibold rounded-full"
                >
                  {{ r.status }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="!summary?.recentCheckIns?.length" class="text-center py-12 text-gray-500 dark:text-gray-400">
        No check-ins for this date
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../services/api';

const selectedDate = ref(new Date().toISOString().split('T')[0]);
const summary = ref(null);

async function loadSummary() {
  try {
    const response = await api.getTeacherDailySummary(selectedDate.value);
    summary.value = response.data;
  } catch (error) {
    console.error('Error loading daily summary:', error);
  }
}

function formatTime(datetime) {
  return new Date(datetime).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  });
}

onMounted(() => {
  loadSummary();
});
</script>
