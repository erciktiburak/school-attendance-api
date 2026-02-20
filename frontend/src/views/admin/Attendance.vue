<template>
  <div>
    <!-- Header -->
    <div class="mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Attendance Logs</h2>
      <p class="text-gray-600">View and manage attendance records</p>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-lg shadow p-4 mb-6">
      <div class="flex gap-4">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-1">Date</label>
          <input
            v-model="selectedDate"
            type="date"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
            @change="loadAttendance"
          />
        </div>
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-1">Search Student</label>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search by name or student number..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-1">Status</label>
          <select
            v-model="filterStatus"
            class="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
          >
            <option value="">All Status</option>
            <option value="Present">Present</option>
            <option value="Late">Late</option>
            <option value="Absent">Absent</option>
            <option value="Excused">Excused</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Total Records</p>
            <p class="text-2xl font-bold text-gray-900">{{ attendanceData?.totalRecords || 0 }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-full">
            <ClipboardDocumentListIcon class="w-6 h-6 text-blue-600" />
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Checked In</p>
            <p class="text-2xl font-bold text-green-600">{{ attendanceData?.checkedIn || 0 }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-full">
            <ArrowRightOnRectangleIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Checked Out</p>
            <p class="text-2xl font-bold text-purple-600">{{ attendanceData?.checkedOut || 0 }}</p>
          </div>
          <div class="p-3 bg-purple-100 rounded-full">
            <ArrowLeftOnRectangleIcon class="w-6 h-6 text-purple-600" />
          </div>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600">Present Rate</p>
            <p class="text-2xl font-bold text-indigo-600">{{ presentRate }}%</p>
          </div>
          <div class="p-3 bg-indigo-100 rounded-full">
            <ChartBarIcon class="w-6 h-6 text-indigo-600" />
          </div>
        </div>
      </div>
    </div>

    <!-- Attendance Table -->
    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Student</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-in Time</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Check-out Time</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Duration</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Location</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="record in filteredRecords" :key="record.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-indigo-100 rounded-full flex items-center justify-center text-indigo-600 font-semibold">
                  {{ record.student?.firstName?.charAt(0) }}{{ record.student?.lastName?.charAt(0) }}
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900">
                    {{ record.student?.firstName }} {{ record.student?.lastName }}
                  </div>
                  <div class="text-sm text-gray-500">{{ record.student?.studentNumber }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ formatDateTime(record.checkInTime) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ record.checkOutTime ? formatDateTime(record.checkOutTime) : '-' }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ calculateDuration(record) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
              {{ record.location }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="getStatusClass(record.status)" class="px-2 py-1 text-xs font-semibold rounded-full">
                {{ record.status }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="filteredRecords.length === 0" class="text-center py-12">
        <ClipboardDocumentListIcon class="w-12 h-12 text-gray-400 mx-auto mb-4" />
        <p class="text-gray-500">No attendance records found</p>
      </div>
    </div>

    <!-- Export Button -->
    <div class="mt-6 flex justify-end">
      <button
        @click="exportToCSV"
        class="flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
      >
        <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
        Export to CSV
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import {
  ClipboardDocumentListIcon,
  ArrowRightOnRectangleIcon,
  ArrowLeftOnRectangleIcon,
  ChartBarIcon,
  ArrowDownTrayIcon,
} from '@heroicons/vue/24/outline';
import api from '../../services/api';

const attendanceData = ref(null);
const selectedDate = ref(new Date().toISOString().split('T')[0]);
const searchQuery = ref('');
const filterStatus = ref('');

const filteredRecords = computed(() => {
  if (!attendanceData.value?.records) return [];
  
  return attendanceData.value.records.filter(record => {
    const matchesSearch = 
      record.student?.firstName?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      record.student?.lastName?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      record.student?.studentNumber?.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    const matchesStatus = !filterStatus.value || record.status === filterStatus.value;
    
    return matchesSearch && matchesStatus;
  });
});

const presentRate = computed(() => {
  if (!attendanceData.value?.records?.length) return 0;
  const presentCount = attendanceData.value.records.filter(r => r.status === 'Present').length;
  return Math.round((presentCount / attendanceData.value.records.length) * 100);
});

async function loadAttendance() {
  try {
    const response = await api.getAttendanceByDate(selectedDate.value);
    attendanceData.value = response.data;
  } catch (error) {
    console.error('Error loading attendance:', error);
  }
}

function formatDateTime(datetime) {
  return new Date(datetime).toLocaleString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  });
}

function calculateDuration(record) {
  if (!record.checkOutTime) return 'In Progress';
  
  const checkIn = new Date(record.checkInTime);
  const checkOut = new Date(record.checkOutTime);
  const minutes = Math.round((checkOut - checkIn) / 60000);
  
  if (minutes < 60) return `${minutes}m`;
  
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  return `${hours}h ${remainingMinutes}m`;
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

function exportToCSV() {
  if (!filteredRecords.value.length) return;
  
  const headers = ['Student Name', 'Student Number', 'Check-in', 'Check-out', 'Duration', 'Location', 'Status'];
  const rows = filteredRecords.value.map(record => [
    `${record.student?.firstName} ${record.student?.lastName}`,
    record.student?.studentNumber,
    formatDateTime(record.checkInTime),
    record.checkOutTime ? formatDateTime(record.checkOutTime) : '-',
    calculateDuration(record),
    record.location,
    record.status,
  ]);
  
  const csvContent = [
    headers.join(','),
    ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
  ].join('\n');
  
  const blob = new Blob([csvContent], { type: 'text/csv' });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = `attendance_${selectedDate.value}.csv`;
  link.click();
  window.URL.revokeObjectURL(url);
}

onMounted(() => {
  loadAttendance();
});
</script>