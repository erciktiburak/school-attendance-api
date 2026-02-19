import { createRouter, createWebHistory } from 'vue-router';

const routes = [
  {
    path: '/',
    redirect: '/admin',
  },
  {
    path: '/admin',
    component: () => import('../views/admin/Layout.vue'),
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: () => import('../views/admin/Dashboard.vue'),
      },
      {
        path: 'students',
        name: 'Students',
        component: () => import('../views/admin/Students.vue'),
      },
      {
        path: 'attendance',
        name: 'Attendance',
        component: () => import('../views/admin/Attendance.vue'),
      },
      {
        path: 'reports',
        name: 'Reports',
        component: () => import('../views/admin/Reports.vue'),
      },
    ],
  },
  {
    path: '/student',
    component: () => import('../views/student/Layout.vue'),
    children: [
      {
        path: '',
        name: 'StudentHome',
        component: () => import('../views/student/Home.vue'),
      },
      {
        path: 'scanner',
        name: 'Scanner',
        component: () => import('../views/student/Scanner.vue'),
      },
      {
        path: 'history',
        name: 'History',
        component: () => import('../views/student/History.vue'),
      },
    ],
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;