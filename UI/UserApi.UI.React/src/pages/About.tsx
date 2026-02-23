import React from 'react';

const About: React.FC = () => {
  return (
    <div style={{ padding: '20px' }}>
      <h1>About Us</h1>
      <p>This project is built using React, TypeScript, and Vite.</p>
      <section style={{ marginTop: '10px' }}>
        <h3>Version Info</h3>
        <ul>
          <li>React: 18+</li>
          <li>TypeScript: 5+</li>
          <li>Vite: Latest</li>
        </ul>
      </section>
    </div>
  );
};

export default About;