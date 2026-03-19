import { createBrowserRouter } from 'react-router-dom';
import Home from './pages/Home';
import About from './pages/About';
import Layout from './components/Layout';
import PermissionsPage from './pages/Permissions';
import Login from './pages/Login';
import TenantManager from '@features/tenants/TenantManager'

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />, // Usually contains your Navbar/Footer
    children: [
      {
        index: true, // This is the default sub-route for "/"
        element: <Home />,
      },
      {
        path: "about",
        element: <About />,
      },
      {
        path: "permissions", // URL will be /permissions
        element: <PermissionsPage />,
      },
      {
        path: "login",
        element: <Login />,
      },
      {
        /* Added the Tenants route. 
           The full URL will be yourdomain.com/tenants 
        */
        path: "tenants",
        element: <TenantManager />,
      }
    ],
  },
]);