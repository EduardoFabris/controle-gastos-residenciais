import React, { useState, useEffect } from 'react';
import { Pessoa, pessoaService } from '../services/apiService';
import './Pessoas.css';

/**
 * Componente para gerenciar pessoas
 * CRUD completo: Criar, Listar, Atualizar e Deletar
 */
export const PessoasComponent: React.FC = () => {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [nome, setNome] = useState('');
  const [idade, setIdade] = useState('');
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  // Carrega a lista de pessoas ao montar o componente
  useEffect(() => {
    carregarPessoas();
  }, []);

  /**
   * Carrega todas as pessoas do backend
   */
  const carregarPessoas = async () => {
    try {
      setLoading(true);
      setErro('');
      const dados = await pessoaService.listarPessoas();
      setPessoas(dados);
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao carregar pessoas');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Cria ou atualiza uma pessoa
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setErro('');
      const idadeNum = parseInt(idade);

      if (editingId) {
        // Atualizar pessoa existente
        const pessoa = await pessoaService.atualizarPessoa(editingId, nome, idadeNum);
        setPessoas(pessoas.map(p => (p.id === editingId ? pessoa : p)));
        setEditingId(null);
      } else {
        // Criar nova pessoa
        const pessoa = await pessoaService.criarPessoa(nome, idadeNum);
        setPessoas([...pessoas, pessoa]);
      }

      setNome('');
      setIdade('');
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao salvar pessoa');
    }
  };

  /**
   * Inicia a edição de uma pessoa
   */
  const handleEdit = (pessoa: Pessoa) => {
    setEditingId(pessoa.id);
    setNome(pessoa.nome);
    setIdade(pessoa.idade.toString());
  };

  /**
   * Deleta uma pessoa
   */
  const handleDelete = async (id: number) => {
    if (window.confirm('Tem certeza que deseja deletar esta pessoa? Todas as suas transações serão deletadas também.')) {
      try {
        setErro('');
        await pessoaService.deletarPessoa(id);
        setPessoas(pessoas.filter(p => p.id !== id));
      } catch (error: any) {
        setErro(error.response?.data?.erro || 'Erro ao deletar pessoa');
      }
    }
  };

  /**
   * Cancela a edição
   */
  const handleCancel = () => {
    setEditingId(null);
    setNome('');
    setIdade('');
    setErro('');
  };

  return (
    <div className="pessoas-container">
      <h2>👥 Gerenciar Pessoas</h2>

      {/* Formulário para criar/editar pessoa */}
      <form onSubmit={handleSubmit} className="pessoa-form">
        <div className="form-group">
          <label htmlFor="nome">Nome:</label>
          <input
            id="nome"
            type="text"
            value={nome}
            onChange={(e) => setNome(e.target.value)}
            placeholder="Digite o nome da pessoa"
            maxLength={200}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="idade">Idade:</label>
          <input
            id="idade"
            type="number"
            value={idade}
            onChange={(e) => setIdade(e.target.value)}
            placeholder="Digite a idade"
            min="0"
            required
          />
        </div>

        <div className="form-buttons">
          <button type="submit" className="btn-primary">
            {editingId ? '✎ Atualizar' : '+ Criar'}
          </button>
          {editingId && (
            <button type="button" onClick={handleCancel} className="btn-secondary">
              ✕ Cancelar
            </button>
          )}
        </div>
      </form>

      {/* Exibe erros */}
      {erro && <div className="alert alert-error">{erro}</div>}

      {/* Lista de pessoas */}
      <div className="pessoas-lista">
        {loading ? (
          <p className="loading">Carregando...</p>
        ) : pessoas.length === 0 ? (
          <p className="empty">Nenhuma pessoa cadastrada</p>
        ) : (
          <table className="tabela-pessoas">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Idade</th>
                <th>Data Criação</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {pessoas.map((pessoa) => (
                <tr key={pessoa.id}>
                  <td>{pessoa.id}</td>
                  <td>{pessoa.nome}</td>
                  <td>{pessoa.idade} anos</td>
                  <td>{new Date(pessoa.dataCriacao).toLocaleDateString('pt-BR')}</td>
                  <td className="acoes">
                    <button onClick={() => handleEdit(pessoa)} className="btn-edit" title="Editar">
                      ✎
                    </button>
                    <button onClick={() => handleDelete(pessoa.id)} className="btn-delete" title="Deletar">
                      🗑️
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <p className="total">Total de pessoas: {pessoas.length}</p>
    </div>
  );
};

export default PessoasComponent;
