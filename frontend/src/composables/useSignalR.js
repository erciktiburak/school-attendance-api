import { ref, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';

const baseUrl = import.meta.env.VITE_API_URL
  ? import.meta.env.VITE_API_URL.replace(/\/api\/?$/, '')
  : 'http://localhost:5141';
const hubUrl = `${baseUrl}/attendanceHub`;

const connection = ref(null);
const isConnected = ref(false);
const notifications = ref([]);

export function useSignalR() {
  const connect = async () => {
    if (connection.value) return;

    connection.value = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect()
      .build();

    connection.value.on('Connected', (connectionId) => {
      console.log('SignalR Connected:', connectionId);
      isConnected.value = true;
    });

    connection.value.on('StudentCheckedIn', (data) => {
      console.log('Student checked in:', data);
      notifications.value.push({
        type: 'checkin',
        message: `${data.studentName} checked in at ${data.location}`,
        data,
        timestamp: new Date(),
      });
    });

    connection.value.on('StudentCheckedOut', (data) => {
      console.log('Student checked out:', data);
      notifications.value.push({
        type: 'checkout',
        message: `${data.studentName} checked out (${data.durationMinutes} min)`,
        data,
        timestamp: new Date(),
      });
    });

    connection.value.on('NewStudentAdded', (data) => {
      console.log('New student added:', data);
      notifications.value.push({
        type: 'newstudent',
        message: `New student added: ${data.name}`,
        data,
        timestamp: new Date(),
      });
    });

    connection.value.onreconnecting(() => {
      isConnected.value = false;
      console.log('SignalR reconnecting...');
    });

    connection.value.onreconnected(() => {
      isConnected.value = true;
      console.log('SignalR reconnected');
    });

    connection.value.onclose(() => {
      isConnected.value = false;
      console.log('SignalR disconnected');
    });

    try {
      await connection.value.start();
      console.log('SignalR connection started');
    } catch (err) {
      console.error('SignalR connection error:', err);
      setTimeout(() => connect(), 5000);
    }
  };

  const disconnect = async () => {
    if (connection.value) {
      await connection.value.stop();
      connection.value = null;
      isConnected.value = false;
    }
  };

  const clearNotifications = () => {
    notifications.value = [];
  };

  return {
    connection,
    isConnected,
    notifications,
    connect,
    disconnect,
    clearNotifications,
  };
}
