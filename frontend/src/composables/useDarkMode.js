import { ref, watch } from 'vue';

const isDark = ref(false);

// Load from localStorage
if (typeof window !== 'undefined') {
  const stored = localStorage.getItem('darkMode');
  isDark.value = stored === 'true';

  // Apply on load
  if (isDark.value) {
    document.documentElement.classList.add('dark');
  }
}

watch(isDark, (newValue) => {
  if (newValue) {
    document.documentElement.classList.add('dark');
    localStorage.setItem('darkMode', 'true');
  } else {
    document.documentElement.classList.remove('dark');
    localStorage.setItem('darkMode', 'false');
  }
});

export function useDarkMode() {
  const toggle = () => {
    isDark.value = !isDark.value;
  };

  return {
    isDark,
    toggle,
  };
}
