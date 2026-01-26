import { createTheme } from "@mui/material/styles";
import { useMemo, useState } from "react";
import { ThemeProvider, CssBaseline } from '@mui/material';
import App from "./App";

export default function Root() {
  const [isDark, setIsDark] = useState(false);

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
      <App isDark={isDark} onToggleTheme={() => setIsDark(p => !p)} />
    </ThemeProvider>
  );
}