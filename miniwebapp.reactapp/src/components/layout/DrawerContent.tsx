import Box from '@mui/material/Box';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';

type NavItem = {
  label: string;
  path: string;
};

const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', path: '/' },
  { label: 'Reports', path: '/reports' },
  { label: 'Settings', path: '/settings' },
];

type DrawerContentProps = {
  title: string;
};

export function DrawerContent({ title }: DrawerContentProps) {
  return (
    <Box>
      <Toolbar>
        <Typography variant="h6">{title}</Typography>
      </Toolbar>
      <List>
        {NAV_ITEMS.map(({ label }) => (
          <ListItem key={label} disablePadding>
            <ListItemButton>
              <ListItemText primary={label} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );
}
