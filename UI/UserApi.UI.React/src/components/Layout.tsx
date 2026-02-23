import { Link, Outlet } from 'react-router-dom';

const Layout = () => {
  return (
    <div>
      <nav>
        <Link to="/">Home</Link> | <Link to="/about">About</Link> | <Link to="/permissions">Permissions</Link>
      </nav>
      <main>
        {/* This is where the Home or About components will appear */}
        <Outlet />
      </main>
    </div>
  );
};

export default Layout;