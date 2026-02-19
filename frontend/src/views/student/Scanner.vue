<template>
  <div class="max-w-md mx-auto">
    <h2 class="text-2xl font-bold text-gray-800 mb-4 text-center">QR Code Check-in</h2>
    <p class="text-gray-600 mb-6 text-center">Scan your student QR code to check in</p>

    <div class="bg-white rounded-lg shadow p-4 mb-4">
      <video ref="videoRef" class="w-full rounded" playsinline></video>
      <div v-if="error" class="mt-2 text-red-600 text-sm">{{ error }}</div>
    </div>

    <div v-if="lastCheckIn" class="bg-green-50 border border-green-200 rounded-lg p-4 text-center">
      <p class="font-medium text-green-800">Check-in successful!</p>
      <p class="text-sm text-green-600">{{ lastCheckIn }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import QrScanner from 'qr-scanner';
import api from '../../services/api';

const videoRef = ref(null);
const scanner = ref(null);
const error = ref('');
const lastCheckIn = ref('');

onMounted(async () => {
  if (!videoRef.value) return;
  try {
    scanner.value = new QrScanner(videoRef.value, (result) => handleScan(result.data), {
      returnDetailedScanResult: true,
      highlightScanRegion: true,
    });
    await scanner.value.start();
  } catch (e) {
    error.value = 'Camera access denied or not available';
  }
});

onUnmounted(() => {
  scanner.value?.destroy();
});

async function handleScan(qrCode) {
  try {
    await api.checkIn(qrCode, 'Main Entrance');
    lastCheckIn.value = `Checked in at ${new Date().toLocaleTimeString()}`;
  } catch (err) {
    error.value = err.response?.data?.message || 'Check-in failed';
  }
}
</script>
