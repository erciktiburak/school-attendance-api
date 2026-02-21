<template>
  <div>
    <div class="mb-6">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white">At-Risk Students</h2>
      <p class="text-gray-600 dark:text-gray-400">Students with low attendance or high late count in the last 30 days</p>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Student</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Department</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Attendance (30d)</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Late Count</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Status</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="s in atRiskStudents" :key="s.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-full flex items-center justify-center text-orange-600 dark:text-orange-400 font-semibold mr-3">
                    {{ s.firstName?.charAt(0) }}{{ s.lastName?.charAt(0) }}
                  </div>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">{{ s.firstName }} {{ s.lastName }}</p>
                    <p class="text-sm text-gray-500 dark:text-gray-400">{{ s.studentNumber }}</p>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">{{ s.department }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-300">{{ s.attendanceCount }} days</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-orange-600 dark:text-orange-400">{{ s.lateCount }}</td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="s.attendanceCount < 15 ? 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-400' : 'bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-400'"
                  class="px-2 py-1 text-xs font-semibold rounded-full"
                >
                  {{ s.attendanceCount < 15 ? 'Low attendance' : 'Frequent late' }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="atRiskStudents.length === 0" class="text-center py-12 text-gray-500 dark:text-gray-400">
        No at-risk students in the last 30 days
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../services/api';

const atRiskStudents = ref([]);

async function loadAtRisk() {
  try {
    const response = await api.getTeacherAtRiskStudents();
    atRiskStudents.value = response.data;
  } catch (error) {
    console.error('Error loading at-risk students:', error);
  }
}

onMounted(() => {
  loadAtRisk();
});
</script>
