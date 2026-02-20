<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white rounded-2xl shadow-xl p-6 text-center">
      <h2 class="text-2xl font-bold text-gray-900 mb-2">QR Code Scanner</h2>
      <p class="text-gray-600">Scan your QR code to check-in or check-out</p>
    </div>

    <!-- Manual Input Option -->
    <div class="bg-white rounded-2xl shadow-xl p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Enter Student Number</h3>
      
      <div class="space-y-4">
        <input
          v-model="studentNumber"
          type="text"
          placeholder="Enter your student number..."
          class="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent text-center text-lg"
        />
        
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Location</label>
          <select
            v-model="location"
            class="w-full px-4 py-3 border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
          >
            <option value="Computer Lab">Computer Lab</option>
            <option value="Library">Library</option>
            <option value="Classroom A">Classroom A</option>
            <option value="Classroom B">Classroom B</option>
            <option value="Main Hall">Main Hall</option>
          </select>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <button
            @click="handleCheckIn"
            :disabled="loading"
            class="flex items-center justify-center px-6 py-4 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors font-semibold disabled:opacity-50"
          >
            <ArrowRightOnRectangleIcon class="w-5 h-5 mr-2" />
            Check In
          </button>
          
          <button
            @click="handleCheckOut"
            :disabled="loading"
            class="flex items-center justify-center px-6 py-4 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors font-semibold disabled:opacity-50"
          >
            <ArrowLeftOnRectangleIcon class="w-5 h-5 mr-2" />
            Check Out
          </button>
        </div>
      </div>
    </div>

    <!-- OR Divider -->
    <div class="flex items-center">
      <div class="flex-1 border-t-2 border-gray-300"></div>
      <span class="px-4 text-gray-500 font-medium">OR</span>
      <div class="flex-1 border-t-2 border-gray-300"></div>
    </div>

    <!-- Camera Scanner (Simulated) -->
    <div class="bg-white rounded-2xl shadow-xl p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4 text-center">Use Camera</h3>
      
      <div class="aspect-square bg-gray-900 rounded-xl flex items-center justify-center mb-4 relative overflow-hidden">
        <div class="absolute inset-0 flex items-center justify-center">
          <div class="w-64 h-64 border-4 border-indigo-500 rounded-lg animate-pulse"></div>
        </div>
        <div class="text-center z-10">
          <QrCodeIcon class="w-16 h-16 text-white mx-auto mb-2" />
          <p class="text-white text-sm">Position QR code in frame</p>
        </div>
      </div>
      
      <p class="text-center text-sm text-gray-500">
        Camera scanning is simulated in this demo. Use manual input above.
      </p>
    </div>

    <!-- Status Message -->
    <div
      v-if="statusMessage"
      :class="[
        'rounded-xl p-4 text-center font-medium',
        statusType === 'success' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
      ]"
    >
      {{ statusMessage }}
    </div>

    <!-- Recent Activity -->
    <div v-if="recentActivity" class="bg-white rounded-2xl shadow-xl p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Last Activity</h3>
      <div class="space-y-3">
        <div class="flex justify-between items-center p-3 bg-gray-50 rounded-lg">
          <span class="text-gray-700">Time</span>
          <span class="font-semibold">{{ formatTime(recentActivity.checkInTime) }}</span>
        </div>
        <div class="flex justify-between items-center p-3 bg-gray-50 rounded-lg">
          <span class="text-gray-700">Location</span>
          <span class="font-semibold">{{ recentActivity.location }}</span>
        </div>
        <div class="flex justify-between items-center p-3 bg-gray-50 rounded-lg">
          <span class="text-gray-700">Status</span>
          <span :class="getStatusClass(recentActivity.status)" class="px-3 py-1 rounded-full text-sm font-semibold">
            {{ recentActivity.status }}
          </span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { QrCodeIcon, ArrowRightOnRectangleIcon, ArrowLeftOnRectangleIcon } from '@heroicons/vue/24/outline';
import api from '../../services/api';

const studentNumber = ref('');
const location = ref('Computer Lab');
const loading = ref(false);
const statusMessage = ref('');
const statusType = ref('success');
const recentActivity = ref(null);

async function handleCheckIn() {
  if (!studentNumber.value) {
    showStatus('Please enter your student number', 'error');
    return;
  }

  try {
    loading.value = true;
    
    // Get student by number to get QR code
    const studentsResponse = await api.getStudents();
    const student = studentsResponse.data.find(s => s.studentNumber === studentNumber.value);
    
    if (!student) {
      showStatus('Student not found', 'error');
      return;
    }

    const response = await api.checkIn(student.qrCode, location.value);
    showStatus(response.data.message || 'Check-in successful!', 'success');
    recentActivity.value = response.data.attendance;
  } catch (error) {
    console.error('Check-in error:', error);
    showStatus(error.response?.data?.message || 'Check-in failed', 'error');
  } finally {
    loading.value = false;
  }
}

async function handleCheckOut() {
  if (!studentNumber.value) {
    showStatus('Please enter your student number', 'error');
    return;
  }

  try {
    loading.value = true;
    
    // Get student by number to get QR code
    const studentsResponse = await api.getStudents();
    const student = studentsResponse.data.find(s => s.studentNumber === studentNumber.value);
    
    if (!student) {
      showStatus('Student not found', 'error');
      return;
    }

    const response = await api.checkOut(student.qrCode);
    showStatus(response.data.message || 'Check-out successful!', 'success');
    recentActivity.value = null;
  } catch (error) {
    console.error('Check-out error:', error);
    showStatus(error.response?.data?.message || 'Check-out failed', 'error');
  } finally {
    loading.value = false;
  }
}

function showStatus(message, type) {
  statusMessage.value = message;
  statusType.value = type;
  setTimeout(() => {
    statusMessage.value = '';
  }, 5000);
}

function formatTime(datetime) {
  return new Date(datetime).toLocaleString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true,
  });
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
</script>