import * as React from 'react';
import {
  Avatar,
  IconButton,
  Menu,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Switch,
  Box,
} from '@mui/material';
import Brightness4Icon from '@mui/icons-material/Brightness4';

type AvatarMenuProps = {
  isDark: boolean;
  onToggleTheme: () => void;
};

export default function AvatarMenu({ isDark, onToggleTheme }: AvatarMenuProps) {
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleOpen = (e: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(e.currentTarget);
  };

  const handleClose = () => setAnchorEl(null);

  return (
    <>
      <IconButton color="inherit" onClick={handleOpen}>
        <Avatar sx={{ width: 32, height: 32 }}>A</Avatar>
      </IconButton>

      <Menu anchorEl={anchorEl} open={open} onClose={handleClose}>
        <MenuItem>
          <ListItemIcon>
            <Brightness4Icon fontSize="small" />
          </ListItemIcon>
          <ListItemText primary="Dark Mode" />
          <Box sx={{ ml: 1 }}>
            <Switch checked={isDark} onChange={onToggleTheme} />
          </Box>
        </MenuItem>
      </Menu>
    </>
  );
}
