<template>
  <div>
    <!-- Header -->
    <div class="mb-6">
      <h2 class="text-2xl font-bold text-gray-900">Reports & Analytics</h2>
      <p class="text-gray-600">Comprehensive attendance analytics and insights</p>
    </div>

    <!-- Report Type Selector -->
    <div class="bg-white rounded-lg shadow p-4 mb-6">
      <div class="flex gap-4">
        <button
          v-for="type in reportTypes"
          :key="type.value"
          @click="selectedReportType = type.value"
          :class="[
            'flex-1 px-4 py-3 rounded-lg font-medium transition-colors',
            selectedReportType === type.value
              ? 'bg-indigo-600 text-white'
              : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
          ]"
        >
          {{ type.label }}
        </button>
      </div>
    </div>

    <!-- Weekly Report -->
    <div v-if="selectedReportType === 'weekly'">
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!-- Weekly Chart -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Weekly Attendance Trend</h3>
          <Line v-if="weeklyChartData" :data="weeklyChartData" :options="lineChartOptions" />
        </div>

        <!-- Weekly Stats -->
        <div class="bg-white rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4">Weekly Statistics</h3>
          <div class="space-y-4">
            <div class="flex justify-between items-center p-3 bg-blue-50 rounded-lg">
              <span class="text-gray-700">Total Attendance</span>
              <span class="text-xl font-bold text-blue-600">{{ weeklyStats?.totalAttendance || 0 }}</span>
            </div>
            <div class="flex justify-between items-center p-3 bg-green-50 rounded-lg">
              <span class="text-gray-700">Average Daily</span>
              <span class="text-xl font-bold text-green-600">{{ calculateWeeklyAverage() }}</span>
            </div>
            <div class="flex justify-between items-center p-3 bg-purple-50 rounded-lg">
              <span class="text-gray-700">Peak Day</span>
              <span class="text-xl font-bold text-purple-600">{{ getPeakDay() }}</span>
            </div>
            <div class="flex justify-between items-center p-3 bg-orange-50 rounded-lg">
              <span class="text-gray-700">Date Range</span>
              <span class="text-sm font-medium text-orange-600">
                {{ formatDate(weeklyStats?.weekStart) }} - {{ formatDate(weeklyStats?.weekEnd) }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Monthly Report -->
    <div v-if="selectedReportType === 'monthly'">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 mb-6">
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Total Attendance</p>
          <p class="text-3xl font-bold text-indigo-600">{{ monthlyStats?.totalAttendance || 0 }}</p>
          <p class="text-sm text-gray-500 mt-2">{{ monthlyStats?.month }}</p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Unique Students</p>
          <p class="text-3xl font-bold text-green-600">{{ monthlyStats?.uniqueAttendees || 0 }}</p>
          <p class="text-sm text-gray-500 mt-2">of {{ monthlyStats?.totalStudents || 0 }} total</p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Participation Rate</p>
          <p class="text-3xl font-bold text-purple-600">{{ monthlyStats?.participationRate || 0 }}%</p>
          <p class="text-sm text-gray-500 mt-2">Overall engagement</p>
        </div>
      </div>

      <div class="bg-white rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold mb-4">Monthly Attendance Pattern</h3>
        <Bar v-if="monthlyChartData" :data="monthlyChartData" :options="barChartOptions" />
      </div>
    </div>

    <!-- Department Report -->
    <div v-if="selectedReportType === 'department'">
      <div class="bg-white rounded-lg shadow p-4 mb-6">
        <select
          v-model="selectedDepartment"
          @change="loadDepartmentStats"
          class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
        >
          <option value="">Select Department</option>
          <option value="Computer Engineering">Computer Engineering</option>
          <option value="Electrical Engineering">Electrical Engineering</option>
          <option value="Mechanical Engineering">Mechanical Engineering</option>
        </select>
      </div>

      <div v-if="departmentStats" class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold">Total Students</h3>
            <UsersIcon class="w-8 h-8 text-indigo-600" />
          </div>
          <p class="text-4xl font-bold text-indigo-600">{{ departmentStats.totalStudents }}</p>
        </div>

        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold">Today's Attendance</h3>
            <ClipboardDocumentCheckIcon class="w-8 h-8 text-green-600" />
          </div>
          <p class="text-4xl font-bold text-green-600">{{ departmentStats.todayAttendance }}</p>
        </div>

        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold">Attendance Rate</h3>
            <ChartBarIcon class="w-8 h-8 text-purple-600" />
          </div>
          <p class="text-4xl font-bold text-purple-600">{{ departmentStats.attendanceRate }}%</p>
        </div>
      </div>
    </div>

    <!-- Student Performance -->
    <div v-if="selectedReportType === 'student'">
      <div class="bg-white rounded-lg shadow p-4 mb-6">
        <div class="flex gap-4">
          <input
            v-model="studentSearchQuery"
            type="text"
            placeholder="Search student by name or number..."
            class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-indigo-500"
          />
          <button
            @click="searchStudent"
            class="px-6 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
          >
            Search
          </button>
        </div>
      </div>

      <div v-if="studentStats" class="space-y-6">
        <!-- Student Info -->
        <div class="bg-white rounded-lg shadow p-6">
          <div class="flex items-center">
            <div class="w-16 h-16 bg-indigo-100 rounded-full flex items-center justify-center text-indigo-600 text-2xl font-semibold">
              {{ studentStats.student.name.split(' ').map(n => n[0]).join('') }}
            </div>
            <div class="ml-4">
              <h3 class="text-xl font-bold text-gray-900">{{ studentStats.student.name }}</h3>
              <p class="text-gray-600">{{ studentStats.student.studentNumber }} • {{ studentStats.student.department }}</p>
            </div>
          </div>
        </div>

        <!-- Student Stats Grid -->
        <div class="grid grid-cols-2 md:grid-cols-3 gap-4">
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">Total Attendance</p>
            <p class="text-2xl font-bold text-indigo-600">{{ studentStats.totalAttendance }}</p>
          </div>
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">This Month</p>
            <p class="text-2xl font-bold text-green-600">{{ studentStats.monthlyAttendance }}</p>
          </div>
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">Present Count</p>
            <p class="text-2xl font-bold text-blue-600">{{ studentStats.presentCount }}</p>
          </div>
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">Late Count</p>
            <p class="text-2xl font-bold text-orange-600">{{ studentStats.lateCount }}</p>
          </div>
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">Attendance Rate</p>
            <p class="text-2xl font-bold text-purple-600">{{ studentStats.attendanceRate }}%</p>
          </div>
          <div class="bg-white rounded-lg shadow p-4">
            <p class="text-sm text-gray-600">Avg Duration</p>
            <p class="text-2xl font-bold text-teal-600">{{ studentStats.averageDurationMinutes }}m</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { Line, Bar } from 'vue-chartjs';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from 'chart.js';
import { UsersIcon, ClipboardDocumentCheckIcon, ChartBarIcon } from '@heroicons/vue/24/outline';
import api from '../../services/api';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, Title, Tooltip, Legend);

const reportTypes = [
  { label: 'Weekly', value: 'weekly' },
  { label: 'Monthly', value: 'monthly' },
  { label: 'Department', value: 'department' },
  { label: 'Student', value: 'student' },
];

const selectedReportType = ref('weekly');
const weeklyStats = ref(null);
const monthlyStats = ref(null);
const departmentStats = ref(null);
const studentStats = ref(null);
const selectedDepartment = ref('');
const studentSearchQuery = ref('');

const weeklyChartData = ref(null);
const monthlyChartData = ref(null);

const lineChartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      display: true,
      position: 'top',
    },
  },
  scales: {
    y: {
      beginAtZero: true,
    },
  },
};

const barChartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      display: false,
    },
  },
  scales: {
    y: {
      beginAtZero: true,
    },
  },
};

async function loadWeeklyStats() {
  try {
    const response = await api.getWeeklyStats();
    weeklyStats.value = response.data;
    
    weeklyChartData.value = {
      labels: response.data.dailyStats.map(day => 
        new Date(day.date).toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
      ),
      datasets: [
        {
          label: 'Present',
          data: response.data.dailyStats.map(day => day.present),
          borderColor: 'rgba(34, 197, 94, 1)',
          backgroundColor: 'rgba(34, 197, 94, 0.1)',
          tension: 0.4,
        },
        {
          label: 'Late',
          data: response.data.dailyStats.map(day => day.late),
          borderColor: 'rgba(251, 146, 60, 1)',
          backgroundColor: 'rgba(251, 146, 60, 0.1)',
          tension: 0.4,
        },
      ],
    };
  } catch (error) {
    console.error('Error loading weekly stats:', error);
  }
}

async function loadMonthlyStats() {
  try {
    const response = await api.getMonthlyStats();
    monthlyStats.value = response.data;
    
    monthlyChartData.value = {
      labels: response.data.dailyStats.map(day => 
        new Date(day.date).toLocaleDateString('en-US', { day: 'numeric' })
      ),
      datasets: [
        {
          label: 'Attendance',
          data: response.data.dailyStats.map(day => day.count),
          backgroundColor: 'rgba(99, 102, 241, 0.8)',
        },
      ],
    };
  } catch (error) {
    console.error('Error loading monthly stats:', error);
  }
}

async function loadDepartmentStats() {
  if (!selectedDepartment.value) return;
  
  try {
    const response = await api.getDepartmentStats(selectedDepartment.value);
    departmentStats.value = response.data;
  } catch (error) {
    console.error('Error loading department stats:', error);
  }
}

async function searchStudent() {
  if (!studentSearchQuery.value) return;
  
  try {
    // First get all students and find by search
    const studentsResponse = await api.getStudents();
    const student = studentsResponse.data.find(s => 
      s.studentNumber.toLowerCase().includes(studentSearchQuery.value.toLowerCase()) ||
      `${s.firstName} ${s.lastName}`.toLowerCase().includes(studentSearchQuery.value.toLowerCase())
    );
    
    if (student) {
      const response = await api.getStudentStats(student.id);
      studentStats.value = response.data;
    } else {
      alert('Student not found');
    }
  } catch (error) {
    console.error('Error searching student:', error);
    alert('Error searching student');
  }
}

function calculateWeeklyAverage() {
  if (!weeklyStats.value?.dailyStats?.length) return 0;
  const total = weeklyStats.value.dailyStats.reduce((sum, day) => sum + day.count, 0);
  return Math.round(total / weeklyStats.value.dailyStats.length);
}

function getPeakDay() {
  if (!weeklyStats.value?.dailyStats?.length) return '-';
  const peak = weeklyStats.value.dailyStats.reduce((max, day) => 
    day.count > max.count ? day : max
  );
  return new Date(peak.date).toLocaleDateString('en-US', { weekday: 'long' });
}

function formatDate(dateString) {
  if (!dateString) return '-';
  return new Date(dateString).toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
}

onMounted(() => {
  loadWeeklyStats();
  loadMonthlyStats();
});
</script>