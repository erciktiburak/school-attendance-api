<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-800 mb-6">Reports</h2>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div class="bg-white rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold mb-2">Weekly Report</h3>
        <p class="text-sm text-gray-500 mb-4">Attendance summary for the last 7 days</p>
        <button
          @click="loadWeeklyReport"
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
        >
          View Report
        </button>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold mb-2">Monthly Report</h3>
        <p class="text-sm text-gray-500 mb-4">Attendance summary for the current month</p>
        <button
          @click="loadMonthlyReport"
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
        >
          View Report
        </button>
      </div>
      <div class="bg-white rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold mb-2">Department Report</h3>
        <p class="text-sm text-gray-500 mb-4">Attendance by department</p>
        <input
          v-model="departmentFilter"
          type="text"
          placeholder="Department name"
          class="w-full px-3 py-2 border rounded mb-2"
        />
        <button
          @click="loadDepartmentReport"
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
        >
          View Report
        </button>
      </div>
    </div>

    <div v-if="reportData" class="mt-8 bg-white rounded-lg shadow p-6">
      <pre class="text-sm overflow-auto">{{ JSON.stringify(reportData, null, 2) }}</pre>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import api from '../../services/api';

const reportData = ref(null);
const departmentFilter = ref('');

async function loadWeeklyReport() {
  try {
    const res = await api.getWeeklyStats();
    reportData.value = res.data;
  } catch (error) {
    console.error('Error loading weekly report:', error);
  }
}

async function loadMonthlyReport() {
  try {
    const res = await api.getMonthlyStats();
    reportData.value = res.data;
  } catch (error) {
    console.error('Error loading monthly report:', error);
  }
}

async function loadDepartmentReport() {
  if (!departmentFilter.value) return;
  try {
    const res = await api.getDepartmentStats(departmentFilter.value);
    reportData.value = res.data;
  } catch (error) {
    console.error('Error loading department report:', error);
  }
}
</script>
