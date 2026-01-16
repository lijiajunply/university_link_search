import { ref, watch } from 'vue';

export function useTheme() {
  const storageKey = 'site-theme';
  const theme = ref(localStorage.getItem(storageKey) || '');

  if (!theme.value) {
    const isDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    theme.value = isDark ? 'dark' : 'light';
  }

  const apply = (val: string) => {
    const html = document.documentElement;
    if (val === 'dark') html.classList.add('dark');
    else html.classList.remove('dark');
    localStorage.setItem(storageKey, val);
  };

  // 监听 theme 变化
  watch(theme, (val) => {
    apply(val);
  }, { immediate: true });

  return {
    theme
  };
}
