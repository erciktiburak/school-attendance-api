import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import api, { setAuthToken } from '../services/api';

const token = ref(localStorage.getItem('token') || '');
const user = ref(JSON.parse(localStorage.getItem('user') || 'null'));

export function useAuth() {
  const router = useRouter();

  const isAuthenticated = computed(() => !!token.value);
  const isAdmin = computed(() => user.value?.role === 'Admin');
  const isTeacher = computed(() => user.value?.role === 'Teacher');
  const isStudent = computed(() => user.value?.role === 'Student');

  const login = async (username, password) => {
    try {
      const response = await api.login({ username, password });

      token.value = response.data.token;
      user.value = {
        username: response.data.username,
        email: response.data.email,
        role: response.data.role,
      };

      localStorage.setItem('token', token.value);
      localStorage.setItem('user', JSON.stringify(user.value));
      setAuthToken(token.value);

      return { success: true };
    } catch (error) {
      console.error('Login error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Login failed',
      };
    }
  };

  const register = async (username, email, password, role = 'Student') => {
    try {
      const response = await api.register({ username, email, password, role });

      token.value = response.data.token;
      user.value = {
        username: response.data.username,
        email: response.data.email,
        role: response.data.role,
      };

      localStorage.setItem('token', token.value);
      localStorage.setItem('user', JSON.stringify(user.value));
      setAuthToken(token.value);

      return { success: true };
    } catch (error) {
      console.error('Register error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Registration failed',
      };
    }
  };

  const logout = () => {
    token.value = '';
    user.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setAuthToken(null);
    router.push('/login');
  };

  if (token.value) {
    setAuthToken(token.value);
  }

  return {
    token,
    user,
    isAuthenticated,
    isAdmin,
    isTeacher,
    isStudent,
    login,
    register,
    logout,
  };
}
