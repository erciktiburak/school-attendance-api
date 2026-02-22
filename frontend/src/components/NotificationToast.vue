<template>
  <div class="fixed top-4 right-4 z-50 space-y-2">
    <transition-group name="slide-fade">
      <div
        v-for="notification in visibleNotifications"
        :key="notification.id"
        :class="[
          'px-4 py-3 rounded-lg shadow-lg flex items-center space-x-3 max-w-sm',
          getNotificationClass(notification.type)
        ]"
      >
        <div :class="getIconClass(notification.type)">
          <component :is="getIcon(notification.type)" class="w-5 h-5" />
        </div>
        <div class="flex-1">
          <p class="text-sm font-medium">{{ notification.message }}</p>
          <p class="text-xs opacity-75">{{ formatTime(notification.timestamp) }}</p>
        </div>
        <button
          @click="removeNotification(notification.id)"
          class="text-current opacity-50 hover:opacity-100"
          type="button"
        >
          ✕
        </button>
      </div>
    </transition-group>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import {
  CheckCircleIcon,
  ArrowRightOnRectangleIcon,
  UserPlusIcon,
} from '@heroicons/vue/24/outline';
import { useSignalR } from '../composables/useSignalR';

const { notifications } = useSignalR();
const visibleNotifications = ref([]);
let notificationId = 0;

watch(
  notifications,
  (newNotifications) => {
    if (newNotifications.length > 0) {
      const latest = newNotifications[newNotifications.length - 1];
      const notification = {
        ...latest,
        id: notificationId++,
      };

      visibleNotifications.value.push(notification);

      setTimeout(() => {
        removeNotification(notification.id);
      }, 5000);
    }
  },
  { deep: true }
);

function removeNotification(id) {
  const index = visibleNotifications.value.findIndex((n) => n.id === id);
  if (index > -1) {
    visibleNotifications.value.splice(index, 1);
  }
}

function getNotificationClass(type) {
  const classes = {
    checkin: 'bg-green-500 text-white',
    checkout: 'bg-blue-500 text-white',
    newstudent: 'bg-purple-500 text-white',
  };
  return classes[type] || 'bg-gray-500 text-white';
}

function getIconClass(type) {
  return 'flex-shrink-0';
}

function getIcon(type) {
  const icons = {
    checkin: CheckCircleIcon,
    checkout: ArrowRightOnRectangleIcon,
    newstudent: UserPlusIcon,
  };
  return icons[type] || CheckCircleIcon;
}

function formatTime(timestamp) {
  return new Date(timestamp).toLocaleTimeString('en-US', {
    hour: '2-digit',
    minute: '2-digit',
  });
}
</script>

<style scoped>
.slide-fade-enter-active {
  transition: all 0.3s ease;
}
.slide-fade-leave-active {
  transition: all 0.3s ease;
}
.slide-fade-enter-from {
  transform: translateX(100%);
  opacity: 0;
}
.slide-fade-leave-to {
  transform: translateX(100%);
  opacity: 0;
}
</style>
