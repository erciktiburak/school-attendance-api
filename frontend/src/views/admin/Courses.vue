<template>
  <div>
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-gray-900 dark:text-white">Courses</h2>
      <button
        @click="openCreateModal"
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600"
      >
        Add Course
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div
        v-for="course in courses"
        :key="course.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow p-6"
      >
        <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-2">{{ course.courseName }}</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">{{ course.courseCode }} • {{ course.credits }} Credits</p>

        <div class="space-y-2 text-sm">
          <p class="text-gray-700 dark:text-gray-300">📅 {{ course.schedule }}</p>
          <p class="text-gray-700 dark:text-gray-300">📍 {{ course.location }}</p>
          <p class="text-gray-700 dark:text-gray-300">👥 {{ course.enrollments?.length || 0 }} students</p>
        </div>

        <div class="mt-4 flex gap-2">
          <button
            @click="viewCourse(course)"
            class="flex-1 px-3 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600 text-sm"
          >
            View
          </button>
          <button
            @click="deleteCourse(course)"
            class="px-3 py-2 bg-red-600 text-white rounded hover:bg-red-700 dark:bg-red-500 dark:hover:bg-red-600 text-sm"
          >
            Delete
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '../../services/api';

const courses = ref([]);

async function loadCourses() {
  try {
    const response = await api.getCourses();
    courses.value = response.data;
  } catch (error) {
    console.error('Error loading courses:', error);
  }
}

function openCreateModal() {
  alert('Create course modal - implement as needed');
}

function viewCourse(course) {
  alert(`View course: ${course.courseName}`);
}

async function deleteCourse(course) {
  if (confirm(`Delete ${course.courseName}?`)) {
    try {
      await api.deleteCourse(course.id);
      loadCourses();
    } catch (error) {
      console.error('Error deleting course:', error);
    }
  }
}

onMounted(() => {
  loadCourses();
});
</script>
