<template>
  <div>
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div>
        <h2 class="text-2xl font-bold text-gray-900 dark:text-white">Students</h2>
        <p class="text-gray-600 dark:text-gray-400">Manage student records and QR codes</p>
      </div>
      <button
        @click="openCreateModal"
        class="flex items-center px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
      >
        <PlusIcon class="w-5 h-5 mr-2" />
        Add Student
      </button>
    </div>

    <!-- Search & Filter -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4 mb-6">
      <div class="flex gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search by name, student number, or email..."
            class="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400"
          />
        </div>
        <select
          v-model="filterDepartment"
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
        >
          <option value="">All Departments</option>
          <option value="Computer Engineering">Computer Engineering</option>
          <option value="Electrical Engineering">Electrical Engineering</option>
          <option value="Mechanical Engineering">Mechanical Engineering</option>
        </select>
        <a
          :href="api.getStudentsExcelUrl()"
          download
          class="flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors whitespace-nowrap"
        >
          <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
          Export Excel
        </a>
      </div>
    </div>

    <!-- Students Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Student</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Student Number</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Department</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">QR Code</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="student in filteredStudents" :key="student.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-indigo-100 dark:bg-indigo-900 rounded-full flex items-center justify-center text-indigo-600 dark:text-indigo-400 font-semibold">
                  {{ student.firstName.charAt(0) }}{{ student.lastName.charAt(0) }}
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ student.firstName }} {{ student.lastName }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ student.email }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-gray-300">
              {{ student.studentNumber }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ student.department }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <button
                @click="showQRCode(student)"
                class="text-indigo-600 dark:text-indigo-400 hover:text-indigo-900 dark:hover:text-indigo-300 flex items-center"
              >
                <QrCodeIcon class="w-5 h-5 mr-1" />
                View QR
              </button>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
              <button
                @click="editStudent(student)"
                class="text-indigo-600 dark:text-indigo-400 hover:text-indigo-900 dark:hover:text-indigo-300 mr-4"
              >
                Edit
              </button>
              <button
                @click="deleteStudent(student)"
                class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300"
              >
                Delete
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-8 max-w-md w-full">
        <h3 class="text-xl font-bold mb-4 dark:text-white">{{ editMode ? 'Edit Student' : 'Add New Student' }}</h3>
        
        <form @submit.prevent="saveStudent">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">First Name</label>
              <input
                v-model="formData.firstName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Last Name</label>
              <input
                v-model="formData.lastName"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Student Number</label>
              <input
                v-model="formData.studentNumber"
                type="text"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Email</label>
              <input
                v-model="formData.email"
                type="email"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Department</label>
              <select
                v-model="formData.department"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="Computer Engineering">Computer Engineering</option>
                <option value="Electrical Engineering">Electrical Engineering</option>
                <option value="Mechanical Engineering">Mechanical Engineering</option>
              </select>
            </div>
          </div>
          
          <div class="flex justify-end space-x-3 mt-6">
            <button
              type="button"
              @click="closeModal"
              class="px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700"
            >
              {{ editMode ? 'Update' : 'Create' }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- QR Code Modal -->
    <div v-if="showQRModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-8 max-w-sm w-full text-center">
        <h3 class="text-xl font-bold mb-4 dark:text-white">{{ selectedStudent?.firstName }} {{ selectedStudent?.lastName }}</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">{{ selectedStudent?.studentNumber }}</p>
        
        <div class="bg-gray-100 dark:bg-gray-700 p-4 rounded-lg mb-4">
          <img
            :src="api.getStudentQRCode(selectedStudent.id)"
            :alt="`QR Code for ${selectedStudent.firstName}`"
            class="mx-auto"
          />
        </div>
        
        <p class="text-xs text-gray-500 dark:text-gray-400 mb-4">Scan this QR code for check-in/check-out</p>
        
        <button
          @click="downloadQRCode"
          class="w-full px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 mb-2"
        >
          Download QR Code
        </button>
        
        <button
          @click="closeQRModal"
          class="w-full px-4 py-2 text-gray-700 dark:text-gray-300 border border-gray-300 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Close
        </button>
      </div>
    </div>
    
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { PlusIcon, QrCodeIcon } from '@heroicons/vue/24/outline';
import api from '../../services/api';

const students = ref([]);
const searchQuery = ref('');
const filterDepartment = ref('');
const showModal = ref(false);
const showQRModal = ref(false);
const editMode = ref(false);
const selectedStudent = ref(null);
const formData = ref({
  firstName: '',
  lastName: '',
  studentNumber: '',
  email: '',
  department: 'Computer Engineering',
});

const filteredStudents = computed(() => {
  return students.value.filter(student => {
    const matchesSearch = 
      student.firstName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      student.lastName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      student.studentNumber.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      student.email.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    const matchesDepartment = 
      !filterDepartment.value || student.department === filterDepartment.value;
    
    return matchesSearch && matchesDepartment;
  });
});

async function loadStudents() {
  try {
    const response = await api.getStudents();
    students.value = response.data;
  } catch (error) {
    console.error('Error loading students:', error);
  }
}

function openCreateModal() {
  editMode.value = false;
  formData.value = {
    firstName: '',
    lastName: '',
    studentNumber: '',
    email: '',
    department: 'Computer Engineering',
  };
  showModal.value = true;
}

function editStudent(student) {
  editMode.value = true;
  formData.value = { ...student };
  showModal.value = true;
}

function closeModal() {
  showModal.value = false;
}

async function saveStudent() {
  try {
    if (editMode.value) {
      await api.updateStudent(formData.value.id, formData.value);
    } else {
      await api.createStudent(formData.value);
    }
    closeModal();
    loadStudents();
  } catch (error) {
    console.error('Error saving student:', error);
    const message = error.response?.data?.message
      || error.response?.data?.detail
      || (typeof error.response?.data === 'string' ? error.response.data : null)
      || error.message;
    alert(message || 'Error saving student. Please try again.');
  }
}

async function deleteStudent(student) {
  if (confirm(`Are you sure you want to delete ${student.firstName} ${student.lastName}?`)) {
    try {
      await api.deleteStudent(student.id);
      loadStudents();
    } catch (error) {
      console.error('Error deleting student:', error);
      alert('Error deleting student. Please try again.');
    }
  }
}

function showQRCode(student) {
  selectedStudent.value = student;
  showQRModal.value = true;
}

function closeQRModal() {
  showQRModal.value = false;
  selectedStudent.value = null;
}

function downloadQRCode() {
  const link = document.createElement('a');
  link.href = api.getStudentQRCode(selectedStudent.value.id);
  link.download = `${selectedStudent.value.studentNumber}_qrcode.png`;
  link.click();
}

onMounted(() => {
  loadStudents();
});
</script>