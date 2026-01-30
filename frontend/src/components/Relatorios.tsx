import React, { useState, useEffect, useCallback } from 'react';
import { transacaoService, ResumoPorPessoa, TotalGeral } from '../services/apiService';
import './Relatorios.css';

/**
 * Componente para exibir relatórios e resumos
 */
export const RelatoriosComponent: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'pessoa' | 'categoria'>('pessoa');
  const [resumoPessoas, setResumoPessoas] = useState<ResumoPorPessoa[]>([]);
  const [resumoCategorias, setResumoCategorias] = useState<any[]>([]);
  const [totalGeral, setTotalGeral] = useState<TotalGeral | null>(null);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  /**
   * Carrega os relatórios baseado na aba ativa
   */
  const carregarRelatorios = useCallback(async () => {
    try {
      setLoading(true);
      setErro('');

      if (activeTab === 'pessoa') {
        const dados = await transacaoService.obterResumoPorPessoa();
        setResumoPessoas(dados.pessoas);
        setTotalGeral(dados.totalGeral);
      } else {
        const dados = await transacaoService.obterResumoPorCategoria();
        setResumoCategorias(dados.categorias);
        setTotalGeral(dados.totalGeral);
      }
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao carregar relatórios');
    } finally {
      setLoading(false);
    }
  }, [activeTab]);

  // Carrega relatórios ao montar o componente
  useEffect(() => {
    carregarRelatorios();
  }, [activeTab, carregarRelatorios]);

  /**
   * Formata valor monetário
   */
  const formatarMoeda = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  /**
   * Formata a cor baseado no saldo (positivo/negativo)
   */
  const getClasseSaldo = (saldo: number) => {
    return saldo >= 0 ? 'saldo-positivo' : 'saldo-negativo';
  };

  return (
    <div className="relatorios-container">
      <h2>📊 Relatórios e Resumos</h2>

      {/* Abas de seleção */}
      <div className="tabs">
        <button
          className={`tab ${activeTab === 'pessoa' ? 'active' : ''}`}
          onClick={() => setActiveTab('pessoa')}
        >
          📈 Por Pessoa
        </button>
        <button
          className={`tab ${activeTab === 'categoria' ? 'active' : ''}`}
          onClick={() => setActiveTab('categoria')}
        >
          📈 Por Categoria
        </button>
      </div>

      {/* Exibe erros */}
      {erro && <div className="alert alert-error">{erro}</div>}

      {loading ? (
        <p className="loading">Carregando relatórios...</p>
      ) : (
        <>
          {/* Relatório por Pessoa */}
          {activeTab === 'pessoa' && (
            <div className="relatorio-pessoa">
              {resumoPessoas.length === 0 ? (
                <p className="empty">Nenhuma pessoa com transações</p>
              ) : (
                <>
                  <table className="tabela-relatorio">
                    <thead>
                      <tr>
                        <th>Pessoa</th>
                        <th>Idade</th>
                        <th>Total Receitas</th>
                        <th>Total Despesas</th>
                        <th>Saldo</th>
                      </tr>
                    </thead>
                    <tbody>
                      {resumoPessoas.map((resumo) => (
                        <tr key={resumo.pessoaId}>
                          <td>{resumo.pessoaNome}</td>
                          <td>{resumo.idade} anos</td>
                          <td className="valor-receita">{formatarMoeda(resumo.totalReceitas)}</td>
                          <td className="valor-despesa">{formatarMoeda(resumo.totalDespesas)}</td>
                          <td className={`valor-saldo ${getClasseSaldo(resumo.saldo)}`}>
                            {formatarMoeda(resumo.saldo)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </>
              )}
            </div>
          )}

          {/* Relatório por Categoria */}
          {activeTab === 'categoria' && (
            <div className="relatorio-categoria">
              {resumoCategorias.length === 0 ? (
                <p className="empty">Nenhuma categoria com transações</p>
              ) : (
                <>
                  <table className="tabela-relatorio">
                    <thead>
                      <tr>
                        <th>Categoria</th>
                        <th>Finalidade</th>
                        <th>Total Receitas</th>
                        <th>Total Despesas</th>
                        <th>Saldo</th>
                      </tr>
                    </thead>
                    <tbody>
                      {resumoCategorias.map((resumo) => (
                        <tr key={resumo.categoriaId}>
                          <td>{resumo.categoriaDescricao}</td>
                          <td>
                            <span className={`finalidade finalidade-${resumo.finalidade === 1 ? 'despesa' : resumo.finalidade === 2 ? 'receita' : 'ambas'}`}>
                              {resumo.finalidade === 1 ? 'Despesa' : resumo.finalidade === 2 ? 'Receita' : 'Ambas'}
                            </span>
                          </td>
                          <td className="valor-receita">{formatarMoeda(resumo.totalReceitas)}</td>
                          <td className="valor-despesa">{formatarMoeda(resumo.totalDespesas)}</td>
                          <td className={`valor-saldo ${getClasseSaldo(resumo.saldo)}`}>
                            {formatarMoeda(resumo.saldo)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </>
              )}
            </div>
          )}

          {/* Total Geral */}
          {totalGeral && (
            <div className="total-geral">
              <h3>📊 Total Geral</h3>
              <div className="resumo-cards">
                <div className="card">
                  <label>Total de Receitas</label>
                  <p className="valor valor-receita">{formatarMoeda(totalGeral.totalReceitas)}</p>
                </div>
                <div className="card">
                  <label>Total de Despesas</label>
                  <p className="valor valor-despesa">{formatarMoeda(totalGeral.totalDespesas)}</p>
                </div>
                <div className={`card ${getClasseSaldo(totalGeral.saldoLiquido)}`}>
                  <label>Saldo Líquido</label>
                  <p className="valor valor-saldo">{formatarMoeda(totalGeral.saldoLiquido)}</p>
                </div>
                <div className="card">
                  <label>Total de Transações</label>
                  <p className="valor">{totalGeral.qtdTransacoes}</p>
                </div>
                <div className="card">
                  <label>Total de Pessoas</label>
                  <p className="valor">{totalGeral.qtdPessoas}</p>
                </div>
                <div className="card">
                  <label>Total de Categorias</label>
                  <p className="valor">{totalGeral.qtdCategorias}</p>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default RelatoriosComponent;
