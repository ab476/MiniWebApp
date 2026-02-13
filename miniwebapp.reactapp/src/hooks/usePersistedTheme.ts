// hooks/usePersistedTheme.ts
import type { PaletteMode } from "@mui/material/styles";
import { useEffect, useState } from "react";

const STORAGE_KEY = "app-theme-mode";

export function usePersistedTheme(defaultValue = false) {
  const [isDark, setIsDark] = useState<boolean>(() => {
    const saved = localStorage.getItem(STORAGE_KEY);
    return saved ? saved === "dark" : defaultValue;
  });

  useEffect(() => {
    const value: PaletteMode = isDark ? "dark" : "light";
    localStorage.setItem(STORAGE_KEY, value);
  }, [isDark]);

  return {
    isDark,
    toggle: () => setIsDark(p => !p),
    setIsDark,
  };
}
