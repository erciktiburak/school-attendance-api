<template>
    <div>
      <!-- SignalR Status -->
      <div
        v-if="isConnected"
        class="mb-4 px-4 py-2 bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-200 rounded-lg flex items-center"
      >
        <div class="w-2 h-2 bg-green-600 rounded-full mr-2 animate-pulse"></div>
        <span class="text-sm">Live updates active</span>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div
          v-for="stat in stats"
          :key="stat.label"
          class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">{{ stat.label }}</p>
              <p class="text-3xl font-bold text-gray-900 dark:text-white">{{ stat.value }}</p>
              <p class="text-sm mt-2" :class="stat.changeColor">
                {{ stat.change }}
              </p>
            </div>
            <div :class="stat.iconBg" class="p-3 rounded-full">
              <component :is="stat.icon" class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>
  
      <!-- Charts Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Weekly Chart -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4 dark:text-white">Weekly Attendance</h3>
          <Bar v-if="weeklyChartData" :data="weeklyChartData" :options="chartOptions" />
        </div>
  
        <!-- Status Pie Chart -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold mb-4 dark:text-white">Today's Status</h3>
          <Doughnut v-if="statusChartData" :data="statusChartData" :options="pieChartOptions" />
        </div>
      </div>
  
      <!-- Recent Activity -->
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
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Location</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Status</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="record in recentActivity" :key="record.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-indigo-100 dark:bg-indigo-900 rounded-full flex items-center justify-center text-indigo-600 dark:text-indigo-400 font-semibold">
                      {{ record.student?.firstName?.charAt(0) }}{{ record.student?.lastName?.charAt(0) }}
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">
                        {{ record.student?.firstName }} {{ record.student?.lastName }}
                      </div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ record.student?.studentNumber }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-300">
                  {{ formatTime(record.checkInTime) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
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
        </div>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted, computed, watch } from 'vue';
  import { Bar, Doughnut } from 'vue-chartjs';
  import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend,
    ArcElement,
  } from 'chart.js';
  import {
    UsersIcon,
    ClipboardDocumentCheckIcon,
    UserGroupIcon,
    ChartBarIcon,
  } from '@heroicons/vue/24/outline';
  import api from '../../services/api';
  import { useSignalR } from '../../composables/useSignalR';

  // Register ChartJS components
  ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend, ArcElement);

  const { isConnected, notifications } = useSignalR();

  const dashboardStats = ref(null);
  const weeklyStats = ref(null);
  const recentActivity = ref([]);
  const loading = ref(true);

  watch(notifications, () => {
    if (notifications.value.length > 0) loadData();
  }, { deep: true });

  const stats = computed(() => {
    if (!dashboardStats.value) return [];
    
    return [
      {
        label: 'Total Students',
        value: dashboardStats.value.totalStudents || 0,
        change: '+12% from last month',
        changeColor: 'text-green-600',
        icon: UsersIcon,
        iconBg: 'bg-blue-500',
      },
      {
        label: 'Today Attendance',
        value: dashboardStats.value.todayAttendance || 0,
        change: `${dashboardStats.value.attendanceRate || 0}% attendance rate`,
        changeColor: 'text-green-600',
        icon: ClipboardDocumentCheckIcon,
        iconBg: 'bg-green-500',
      },
      {
        label: 'Active Check-ins',
        value: dashboardStats.value.activeCheckIns || 0,
        change: 'Currently in campus',
        changeColor: 'text-blue-600',
        icon: UserGroupIcon,
        iconBg: 'bg-purple-500',
      },
      {
        label: 'Weekly Average',
        value: calculateWeeklyAverage(),
        change: 'Last 7 days',
        changeColor: 'text-gray-600',
        icon: ChartBarIcon,
        iconBg: 'bg-orange-500',
      },
    ];
  });
  
  const weeklyChartData = computed(() => {
    if (!weeklyStats.value?.dailyStats) return null;
    
    const labels = weeklyStats.value.dailyStats.map(day => 
      new Date(day.date).toLocaleDateString('en-US', { weekday: 'short' })
    );
    const data = weeklyStats.value.dailyStats.map(day => day.count);
    
    return {
      labels,
      datasets: [
        {
          label: 'Attendance',
          data,
          backgroundColor: 'rgba(99, 102, 241, 0.8)',
          borderColor: 'rgba(99, 102, 241, 1)',
          borderWidth: 1,
        },
      ],
    };
  });
  
  const statusChartData = computed(() => {
    if (!recentActivity.value.length) return null;
    
    const statusCount = recentActivity.value.reduce((acc, record) => {
      acc[record.status] = (acc[record.status] || 0) + 1;
      return acc;
    }, {});
    
    return {
      labels: Object.keys(statusCount),
      datasets: [
        {
          data: Object.values(statusCount),
          backgroundColor: [
            'rgba(34, 197, 94, 0.8)',
            'rgba(251, 146, 60, 0.8)',
            'rgba(239, 68, 68, 0.8)',
            'rgba(168, 85, 247, 0.8)',
          ],
        },
      ],
    };
  });
  
  const chartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        display: false,
      },
    },
  };
  
  const pieChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    plugins: {
      legend: {
        position: 'bottom',
      },
    },
  };
  
  function calculateWeeklyAverage() {
    if (!weeklyStats.value?.dailyStats) return 0;
    const total = weeklyStats.value.dailyStats.reduce((sum, day) => sum + day.count, 0);
    return Math.round(total / weeklyStats.value.dailyStats.length);
  }
  
  function formatTime(datetime) {
    return new Date(datetime).toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit',
    });
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
  
  async function loadData() {
    try {
      loading.value = true;
      
      const [dashboardRes, weeklyRes, todayRes] = await Promise.all([
        api.getDashboardStats(),
        api.getWeeklyStats(),
        api.getTodayAttendance(),
      ]);
      
      dashboardStats.value = dashboardRes.data;
      weeklyStats.value = weeklyRes.data;
      recentActivity.value = todayRes.data.records.slice(0, 10);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      loading.value = false;
    }
  }
  
  onMounted(() => {
    loadData();
  });
  </script>