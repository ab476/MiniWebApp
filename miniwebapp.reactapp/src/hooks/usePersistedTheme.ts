// hooks/usePersistedTheme.ts
import { useEffect, useState } from "react";

const STORAGE_KEY = "app-theme-mode";

export function usePersistedTheme(defaultValue = false) {
  const [isDark, setIsDark] = useState<boolean>(() => {
    const saved = localStorage.getItem(STORAGE_KEY);
    return saved ? saved === "dark" : defaultValue;
  });

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, isDark ? "dark" : "light");
  }, [isDark]);

  return {
    isDark,
    toggle: () => setIsDark(p => !p),
    setIsDark,
  };
}
