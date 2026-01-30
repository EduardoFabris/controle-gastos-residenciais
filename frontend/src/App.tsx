import React, { useState } from 'react';
import PessoasComponent from './components/Pessoas';
import CategoriasComponent from './components/Categorias';
import TransacoesComponent from './components/Transacoes';
import RelatoriosComponent from './components/Relatorios';
import './App.css';

type NavItem = 'pessoas' | 'categorias' | 'transacoes' | 'relatorios';

/**
 * Componente principal da aplicação
 * Renderiza a navegação e o componente selecionado
 */
function App() {
  const [activeNav, setActiveNav] = useState<NavItem>('pessoas');

  return (
    <div className="app">
      {/* Header */}
      <header className="header">
        <div className="header-content">
          <h1>💼 Sistema de Controle de Gastos Residenciais</h1>
          <p className="subtitle">Gerencie suas finanças pessoais com facilidade</p>
        </div>
      </header>

      <div className="app-content">
        {/* Sidebar Navigation */}
        <nav className="sidebar">
          <ul className="nav-list">
            <li>
              <button
                className={`nav-item ${activeNav === 'pessoas' ? 'active' : ''}`}
                onClick={() => setActiveNav('pessoas')}
                title="Gerenciar Pessoas"
              >
                👥 Pessoas
              </button>
            </li>
            <li>
              <button
                className={`nav-item ${activeNav === 'categorias' ? 'active' : ''}`}
                onClick={() => setActiveNav('categorias')}
                title="Gerenciar Categorias"
              >
                📂 Categorias
              </button>
            </li>
            <li>
              <button
                className={`nav-item ${activeNav === 'transacoes' ? 'active' : ''}`}
                onClick={() => setActiveNav('transacoes')}
                title="Gerenciar Transações"
              >
                💰 Transações
              </button>
            </li>
            <li>
              <button
                className={`nav-item ${activeNav === 'relatorios' ? 'active' : ''}`}
                onClick={() => setActiveNav('relatorios')}
                title="Ver Relatórios"
              >
                📊 Relatórios
              </button>
            </li>
          </ul>
        </nav>

        {/* Main Content */}
        <main className="main-content">
          {activeNav === 'pessoas' && <PessoasComponent />}
          {activeNav === 'categorias' && <CategoriasComponent />}
          {activeNav === 'transacoes' && <TransacoesComponent />}
          {activeNav === 'relatorios' && <RelatoriosComponent />}
        </main>
      </div>

      {/* Footer */}
      <footer className="footer">
        <p>&copy; 2026 Sistema de Controle de Gastos. Desenvolvido por Eduardo Fabris</p>
      </footer>
    </div>
  );
}

export default App;
