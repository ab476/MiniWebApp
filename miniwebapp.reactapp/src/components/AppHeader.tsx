import { AppBar, Toolbar, Typography, IconButton, Box } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import AvatarMenu from './AvatarMenu';

type AppHeaderProps = {
  title: string;
  drawerWidth: number;
  onMenuClick: () => void;
  isDark: boolean;
  onToggleTheme: () => void;
};

export default function AppHeader({
  title,
  drawerWidth,
  onMenuClick,
  isDark,
  onToggleTheme,
}: AppHeaderProps) {
  return (
    <AppBar
      position="fixed"
      sx={{
        width: { sm: `calc(100% - ${drawerWidth}px)` },
        ml: { sm: `${drawerWidth}px` },
      }}
    >
      <Toolbar>
        <IconButton
          color="inherit"
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 2, display: { sm: 'none' } }}
        >
          <MenuIcon />
        </IconButton>

        <Typography variant="h6" noWrap sx={{ flexGrow: 1 }}>
          {title}
        </Typography>

        {/* Right side */}
        <Box>
          <AvatarMenu isDark={isDark} onToggleTheme={onToggleTheme} />
        </Box>
      </Toolbar>
    </AppBar>
  );
}
