import { Link as RouterLink, Outlet } from 'react-router-dom';
import { AppBar, Toolbar, Button, Container, Box } from '@mui/material';

const Layout = () => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <AppBar position="static">
        <Toolbar>
          <Button color="inherit" component={RouterLink} to="/" data-testid="home-link">
            Home
          </Button>
          <Button color="inherit" component={RouterLink} to="/about" data-testid="about-link">
            About
          </Button>
          <Button color="inherit" component={RouterLink} to="/permissions" data-testid="permissions-link">
            Permissions
          </Button>
          <Button color="inherit" component={RouterLink} to="/login" data-testid="login-link">
            Login
          </Button>
        </Toolbar>
      </AppBar>
      <Container component="main" sx={{ flexGrow: 1, py: 3 }} data-testid="main-content" maxWidth={false}>
        <Outlet />
      </Container>
    </Box>
  );
};

export default Layout;