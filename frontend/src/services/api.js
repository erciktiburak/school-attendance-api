import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5141/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export function setAuthToken(token) {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
}

export default {
  // Auth
  login(credentials) {
    return api.post('/auth/login', credentials);
  },
  register(data) {
    return api.post('/auth/register', data);
  },

  // Students
  getStudents() {
    return api.get('/student');
  },
  getStudent(id) {
    return api.get(`/student/${id}`);
  },
  getStudentByQR(qrCode) {
    return api.get(`/student/qr/${qrCode}`);
  },
  createStudent(student) {
    return api.post('/student', student);
  },
  updateStudent(id, student) {
    return api.put(`/student/${id}`, student);
  },
  deleteStudent(id) {
    return api.delete(`/student/${id}`);
  },
  getStudentQRCode(id) {
    return `${API_BASE_URL}/student/${id}/qrcode`;
  },

  // Attendance
  checkIn(qrCode, location) {
    return api.post('/attendance/checkin', { qrCode, location });
  },
  checkOut(qrCode) {
    return api.post('/attendance/checkout', { qrCode });
  },
  getTodayAttendance() {
    return api.get('/attendance/today');
  },
  getStudentAttendance(studentId) {
    return api.get(`/attendance/student/${studentId}`);
  },
  getAttendanceByDate(date) {
    return api.get(`/attendance/date/${date}`);
  },

  // Statistics
  getDashboardStats() {
    return api.get('/statistics/dashboard');
  },
  getWeeklyStats() {
    return api.get('/statistics/weekly');
  },
  getMonthlyStats() {
    return api.get('/statistics/monthly');
  },
  getStudentStats(studentId) {
    return api.get(`/statistics/student/${studentId}`);
  },
  getDepartmentStats(department) {
    return api.get(`/statistics/department/${department}`);
  },

  // Excel Export
  getAttendanceExcelUrl(date) {
    return `${API_BASE_URL}/export/attendance?date=${date}`;
  },
  getStudentsExcelUrl() {
    return `${API_BASE_URL}/export/students`;
  },
};