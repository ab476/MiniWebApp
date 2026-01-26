import { createTheme } from '@mui/material/styles';

const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#9c27b0',
    },
    background: {
      default: '#f5f7fa',
    },
  },
  typography: {
    fontFamily: `'Inter', 'Roboto', 'Arial', sans-serif`,
    h1: {
      fontSize: '2rem',
      fontWeight: 600,
    },
  },
  shape: {
    borderRadius: 8,
  },
});

export default theme;

export const lightTheme = createTheme({ /* ... */ });
export const darkTheme = createTheme({ palette: { mode: 'dark' } });
