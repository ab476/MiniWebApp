import { createTheme } from "@mui/material/styles";
import { useMemo } from "react";
import { ThemeProvider, CssBaseline } from '@mui/material';
import App from "./App";
import { usePersistedTheme } from "./hooks/usePersistedTheme";

export default function Root() {
  const { isDark, toggle } = usePersistedTheme();

  const theme = useMemo(
    () =>
      createTheme({
        palette: {
          mode: isDark ? 'dark' : 'light',
        },
      }),
    [isDark]
  );

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <App isDark={isDark} onToggleTheme={toggle} />
    </ThemeProvider>
  );
}