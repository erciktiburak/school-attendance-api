<template>
  <div>
    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Students</p>
            <p class="text-3xl font-bold text-gray-900 dark:text-white">{{ dashboardData?.totalStudents || 0 }}</p>
          </div>
          <UsersIcon class="w-12 h-12 text-blue-500" />
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Today Present</p>
            <p class="text-3xl font-bold text-green-600 dark:text-green-400">{{ dashboardData?.todayAttendance || 0 }}</p>
          </div>
          <CheckCircleIcon class="w-12 h-12 text-green-500" />
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Late Today</p>
            <p class="text-3xl font-bold text-orange-600 dark:text-orange-400">{{ dashboardData?.lateCount || 0 }}</p>
          </div>
          <ClockIcon class="w-12 h-12 text-orange-500" />
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Absent Today</p>
            <p class="text-3xl font-bold text-red-600 dark:text-red-400">{{ dashboardData?.absentCount || 0 }}</p>
          </div>
          <XCircleIcon class="w-12 h-12 text-red-500" />
        </div>
      </div>
    </div>

    <!-- Late & Absent Students -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
        <div class="p-6 border-b dark:border-gray-700">
          <h3 class="text-lg font-semibold dark:text-white">Late Students Today</h3>
        </div>
        <div class="p-6">
          <div v-if="dashboardData?.lateStudents?.length" class="space-y-3">
            <div
              v-for="record in dashboardData.lateStudents"
              :key="record.id"
              class="flex items-center justify-between p-3 bg-orange-50 dark:bg-orange-900/20 rounded-lg"
            >
              <div class="flex items-center">
                <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-full flex items-center justify-center text-orange-600 dark:text-orange-400 font-semibold mr-3">
                  {{ record.student?.firstName?.charAt(0) }}{{ record.student?.lastName?.charAt(0) }}
                </div>
                <div>
                  <p class="font-medium text-gray-900 dark:text-white">
                    {{ record.student?.firstName }} {{ record.student?.lastName }}
                  </p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">{{ record.student?.studentNumber }}</p>
                </div>
              </div>
              <span class="text-sm text-gray-600 dark:text-gray-400">
                {{ formatTime(record.checkInTime) }}
              </span>
            </div>
          </div>
          <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400">
            No late students today
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
        <div class="p-6 border-b dark:border-gray-700">
          <h3 class="text-lg font-semibold dark:text-white">Absent Students Today</h3>
        </div>
        <div class="p-6">
          <div v-if="dashboardData?.absentStudents?.length" class="space-y-3 max-h-96 overflow-y-auto">
            <div
              v-for="student in dashboardData.absentStudents"
              :key="student.id"
              class="flex items-center justify-between p-3 bg-red-50 dark:bg-red-900/20 rounded-lg"
            >
              <div class="flex items-center flex-1 min-w-0">
                <div class="w-10 h-10 flex-shrink-0 bg-red-100 dark:bg-red-900 rounded-full flex items-center justify-center text-red-600 dark:text-red-400 font-semibold mr-3">
                  {{ student.firstName?.charAt(0) }}{{ student.lastName?.charAt(0) }}
                </div>
                <div class="min-w-0">
                  <p class="font-medium text-gray-900 dark:text-white truncate">
                    {{ student.firstName }} {{ student.lastName }}
                  </p>
                  <p class="text-sm text-gray-500 dark:text-gray-400">{{ student.studentNumber }}</p>
                </div>
              </div>
              <div class="flex gap-2 flex-shrink-0 ml-2">
                <button
                  @click="markAbsent(student.id, false)"
                  class="px-3 py-1 text-xs bg-red-600 text-white rounded hover:bg-red-700"
                >
                  Absent
                </button>
                <button
                  @click="markAbsent(student.id, true)"
                  class="px-3 py-1 text-xs bg-purple-600 text-white rounded hover:bg-purple-700"
                >
                  Excused
                </button>
              </div>
            </div>
          </div>
          <div v-else class="text-center py-8 text-gray-500 dark:text-gray-400">
            All students present
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import {
  UsersIcon,
  CheckCircleIcon,
  ClockIcon,
  XCircleIcon,
} from '@heroicons/vue/24/outline';
import api from '../../services/api';

const dashboardData = ref(null);

async function loadDashboard() {
  try {
    const response = await api.getTeacherDashboard();
    dashboardData.value = response.data;
  } catch (error) {
    console.error('Error loading teacher dashboard:', error);
  }
}

async function markAbsent(studentId, isExcused) {
  try {
    await api.markStudentAbsent(studentId, isExcused);
    await loadDashboard();
    alert(`Student marked as ${isExcused ? 'excused' : 'absent'}`);
  } catch (error) {
    console.error('Error marking absent:', error);
    alert(error.response?.data?.message || 'Failed to mark student');
  }
}

function formatTime(datetime) {
  return new Date(datetime).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
  });
}

onMounted(() => {
  loadDashboard();
});
</script>
