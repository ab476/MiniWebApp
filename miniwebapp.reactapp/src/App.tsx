import './App.css';
import Layout from './components/layout/Layout';

type AppProps = {
  isDark: boolean;
  onToggleTheme: () => void;
};

export default function App({ isDark, onToggleTheme }: AppProps) {
  return (
    <Layout isDark={isDark} onToggleTheme={onToggleTheme}>
      <h2>Dashboard</h2>
      <p>Your content goes here.</p>
    </Layout>
  );
}
