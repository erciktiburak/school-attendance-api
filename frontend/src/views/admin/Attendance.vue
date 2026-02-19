<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-800">Attendance</h2>
      <input
        v-model="selectedDate"
        type="date"
        class="px-4 py-2 border rounded-lg"
      />
    </div>

    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Student</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-in</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-out</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="record in attendanceRecords" :key="record.id" class="hover:bg-gray-50">
            <td class="px-6 py-4">
              <div class="font-medium text-gray-900">
                {{ record.student?.firstName }} {{ record.student?.lastName }}
              </div>
              <div class="text-sm text-gray-500">{{ record.student?.studentNumber }}</div>
            </td>
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
      <div v-else-if="!attendanceRecords.length" class="p-8 text-center text-gray-500">No attendance records for this date.</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue';
import api from '../../services/api';

const attendanceRecords = ref([]);
const loading = ref(true);
const selectedDate = ref(new Date().toISOString().split('T')[0]);

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

async function loadAttendance() {
  try {
    loading.value = true;
    const res = await api.getAttendanceByDate(selectedDate.value);
    attendanceRecords.value = res.data?.records || res.data || [];
  } catch (error) {
    console.error('Error loading attendance:', error);
    attendanceRecords.value = [];
  } finally {
    loading.value = false;
  }
}

onMounted(() => loadAttendance());
watch(selectedDate, () => loadAttendance());
</script>
