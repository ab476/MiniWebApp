import { Box, Drawer, Toolbar } from '@mui/material';
import AppHeader from './AppHeader';
import { DrawerContent } from './DrawerContent';
import { useCallback, useState, type PropsWithChildren } from 'react';

const drawerWidth = 240;
type LayoutProps = PropsWithChildren<{
  isDark: boolean;
  onToggleTheme: () => void;
}>;

export default function Layout({ children, isDark, onToggleTheme }: LayoutProps) {
  const [mobileOpen, setMobileOpen] = useState(false);

  const handleDrawerToggle = useCallback(() => {
    setMobileOpen((prev) => !prev);
  }, []);

  const drawer = <DrawerContent title="App Name" />;

  return (
    <Box sx={{ display: 'flex' }}>
      {/* Top Bar */}
      <AppHeader
        title="Page Title"
        drawerWidth={drawerWidth}
        onMenuClick={handleDrawerToggle}
        isDark={isDark}
        onToggleTheme={onToggleTheme}
      />

      {/* Sidebar */}
      <Box component="nav" sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>

        <Drawer
          variant="permanent"
          open
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
      </Box>

      {/* Main Content */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
        }}
      >
        <Toolbar />
        {children}
      </Box>
    </Box>
  );
}
