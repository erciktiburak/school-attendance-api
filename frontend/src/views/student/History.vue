<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-800 mb-6">My Attendance History</h2>

    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-in</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-out</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="record in history" :key="record.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 text-sm text-gray-900">{{ formatDate(record.checkInTime) }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ formatTime(record.checkInTime) }}</td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ record.checkOutTime ? formatTime(record.checkOutTime) : '-' }}</td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(record.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                {{ record.status }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="loading" class="p-8 text-center text-gray-500">Loading...</div>
      <div v-else-if="!history.length" class="p-8 text-center text-gray-500">
        No attendance history. Enter your student ID to view records.
      </div>
    </div>

    <div class="mt-4 flex gap-2">
      <input
        v-model="studentId"
        type="number"
        placeholder="Student ID"
        class="px-3 py-2 border rounded"
      />
      <button
        @click="loadHistory"
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
      >
        Load History
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import api from '../../services/api';

const history = ref([]);
const loading = ref(false);
const studentId = ref('');

function formatDate(datetime) {
  return datetime ? new Date(datetime).toLocaleDateString() : '-';
}

function formatTime(datetime) {
  return datetime ? new Date(datetime).toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' }) : '-';
}

function getStatusClass(status) {
  const classes = {
    Present: 'bg-green-100 text-green-800',
    Late: 'bg-orange-100 text-orange-800',
    Absent: 'bg-red-100 text-red-800',
    Excused: 'bg-purple-100 text-purple-800',
  };
  return classes[status] || 'bg-gray-100 text-gray-800';
}

async function loadHistory() {
  if (!studentId.value) return;
  try {
    loading.value = true;
    const res = await api.getStudentAttendance(studentId.value);
    history.value = res.data?.records || res.data || [];
  } catch (error) {
    console.error('Error loading history:', error);
    history.value = [];
  } finally {
    loading.value = false;
  }
}
</script>
