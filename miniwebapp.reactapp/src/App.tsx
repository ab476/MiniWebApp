import './App.css'
import Layout from './components/Layout'

type AppProps = {
  isDark: boolean;
  onToggleTheme: () => void;
};

function App({ isDark, onToggleTheme }: AppProps) {
  return (
    <Layout isDark={isDark} onToggleTheme={onToggleTheme}>
      <h2>Dashboard</h2>
      <p>Your content goes here.</p>
    </Layout>
  );
}

export default App
