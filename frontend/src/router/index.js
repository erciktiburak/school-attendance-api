import { createRouter, createWebHistory } from 'vue-router';
import { useAuth } from '../composables/useAuth';

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/Login.vue'),
  },
  {
    path: '/',
    redirect: '/admin',
  },
  {
    path: '/admin',
    component: () => import('../views/admin/Layout.vue'),
    meta: { requiresAuth: true },
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
    meta: { requiresAuth: true },
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

router.beforeEach((to, from, next) => {
  const { isAuthenticated } = useAuth();

  if (to.meta.requiresAuth && !isAuthenticated.value) {
    next('/login');
  } else if (to.path === '/login' && isAuthenticated.value) {
    next('/admin');
  } else {
    next();
  }
});

export default router;
