import { ref } from 'vue';

const isDark = ref(false);

function applyDarkMode(value) {
  if (typeof document === 'undefined') return;
  const html = document.documentElement;
  if (value) {
    html.classList.add('dark');
    localStorage.setItem('darkMode', 'true');
  } else {
    html.classList.remove('dark');
    localStorage.setItem('darkMode', 'false');
  }
}

// Load from localStorage and apply immediately
if (typeof window !== 'undefined') {
  const stored = localStorage.getItem('darkMode');
  isDark.value = stored === 'true';
  applyDarkMode(isDark.value);
}

export function useDarkMode() {
  const toggle = () => {
    isDark.value = !isDark.value;
    applyDarkMode(isDark.value);
  };

  return {
    isDark,
    toggle,
  };
}
