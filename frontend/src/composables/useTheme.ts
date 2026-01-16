import { storeToRefs } from 'pinia';
import { useThemeStore } from '../stores/ThemeStore';

export function useTheme() {
  const store = useThemeStore();
  const { theme, isDarkMode, naiveTheme } = storeToRefs(store);
  const { toggleTheme } = store;

  return {
    theme,
    isDarkMode,
    naiveTheme,
    toggleTheme
  };
}
