<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-800">Students</h2>
      <button
        @click="showAddModal = true"
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition-colors"
      >
        Add Student
      </button>
    </div>

    <div class="bg-white rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Student</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Department</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Email</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200">
          <tr v-for="student in students" :key="student.id" class="hover:bg-gray-50">
            <td class="px-6 py-4">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-indigo-100 rounded-full flex items-center justify-center text-indigo-600 font-semibold">
                  {{ student.firstName?.charAt(0) }}{{ student.lastName?.charAt(0) }}
                </div>
                <div class="ml-4">
                  <div class="font-medium text-gray-900">{{ student.firstName }} {{ student.lastName }}</div>
                  <div class="text-sm text-gray-500">{{ student.studentNumber }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 text-sm text-gray-900">{{ student.department }}</td>
            <td class="px-6 py-4 text-sm text-gray-500">{{ student.email }}</td>
            <td class="px-6 py-4">
              <div class="flex gap-2">
                <a :href="api.getStudentQRCode(student.id)" target="_blank" class="text-indigo-600 hover:text-indigo-800">QR</a>
                <button @click="editStudent(student)" class="text-blue-600 hover:text-blue-800">Edit</button>
                <button @click="deleteStudent(student)" class="text-red-600 hover:text-red-800">Delete</button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-if="loading" class="p-8 text-center text-gray-500">Loading...</div>
      <div v-else-if="!students.length" class="p-8 text-center text-gray-500">No students found.</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../services/api';

const students = ref([]);
const loading = ref(true);
const showAddModal = ref(false);

async function loadStudents() {
  try {
    loading.value = true;
    const res = await api.getStudents();
    students.value = res.data;
  } catch (error) {
    console.error('Error loading students:', error);
  } finally {
    loading.value = false;
  }
}

function editStudent(student) {
  // TODO: Implement edit modal
  console.log('Edit', student);
}

async function deleteStudent(student) {
  if (!confirm(`Delete ${student.firstName} ${student.lastName}?`)) return;
  try {
    await api.deleteStudent(student.id);
    await loadStudents();
  } catch (error) {
    console.error('Error deleting student:', error);
  }
}

onMounted(() => {
  loadStudents();
});
</script>
