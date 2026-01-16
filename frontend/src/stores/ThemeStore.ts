import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';
import { darkTheme } from 'naive-ui';

export const useThemeStore = defineStore('theme', () => {
  const storageKey = 'site-theme';
  
  // 迁移逻辑
  const legacyTheme = localStorage.getItem('theme');
  if (legacyTheme) {
    localStorage.setItem(storageKey, legacyTheme);
    localStorage.removeItem('theme');
  }

  // 初始化状态
  const savedTheme = localStorage.getItem(storageKey);
  const systemDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  
  const theme = ref<string>(savedTheme || (systemDark ? 'dark' : 'light'));

  // 计算属性
  const isDarkMode = computed(() => theme.value === 'dark');
  const naiveTheme = computed(() => theme.value === 'dark' ? darkTheme : null);

  // 动作
  const toggleTheme = () => {
    theme.value = theme.value === 'dark' ? 'light' : 'dark';
  };

  const setTheme = (newTheme: 'dark' | 'light') => {
    theme.value = newTheme;
  };

  // 监听并应用副作用
  watch(theme, (val) => {
    const html = document.documentElement;
    if (val === 'dark') {
      html.classList.add('dark');
    } else {
      html.classList.remove('dark');
    }
    localStorage.setItem(storageKey, val);
  }, { immediate: true });

  return {
    theme,
    isDarkMode,
    naiveTheme,
    toggleTheme,
    setTheme
  };
});
