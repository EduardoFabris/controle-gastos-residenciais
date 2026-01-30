import React, { useState, useEffect } from 'react';
import { Categoria, categoriaService, EnumFinalidade } from '../services/apiService';
import './Categorias.css';

const FINALIDADE_LABELS: { [key: number]: string } = {
  1: 'Despesa',
  2: 'Receita',
  3: 'Ambas',
};

/**
 * Componente para gerenciar categorias
 * CRUD completo: Criar, Listar, Atualizar e Deletar
 */
export const CategoriasComponent: React.FC = () => {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [descricao, setDescricao] = useState('');
  const [finalidade, setFinalidade] = useState<EnumFinalidade>(EnumFinalidade.Ambas);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');

  // Carrega a lista de categorias ao montar o componente
  useEffect(() => {
    carregarCategorias();
  }, []);

  /**
   * Carrega todas as categorias do backend
   */
  const carregarCategorias = async () => {
    try {
      setLoading(true);
      setErro('');
      const dados = await categoriaService.listarCategorias();
      setCategorias(dados);
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao carregar categorias');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Cria ou atualiza uma categoria
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setErro('');

      if (editingId) {
        // Atualizar categoria existente
        const categoria = await categoriaService.atualizarCategoria(editingId, descricao, finalidade);
        setCategorias(categorias.map(c => (c.id === editingId ? categoria : c)));
        setEditingId(null);
      } else {
        // Criar nova categoria
        const categoria = await categoriaService.criarCategoria(descricao, finalidade);
        setCategorias([...categorias, categoria]);
      }

      setDescricao('');
      setFinalidade(EnumFinalidade.Ambas);
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao salvar categoria');
    }
  };

  /**
   * Inicia a edição de uma categoria
   */
  const handleEdit = (categoria: Categoria) => {
    setEditingId(categoria.id);
    setDescricao(categoria.descricao);
    setFinalidade(categoria.finalidade);
  };

  /**
   * Deleta uma categoria
   */
  const handleDelete = async (id: number) => {
    if (window.confirm('Tem certeza que deseja deletar esta categoria?')) {
      try {
        setErro('');
        await categoriaService.deletarCategoria(id);
        setCategorias(categorias.filter(c => c.id !== id));
      } catch (error: any) {
        setErro(error.response?.data?.erro || 'Erro ao deletar categoria');
      }
    }
  };

  /**
   * Cancela a edição
   */
  const handleCancel = () => {
    setEditingId(null);
    setDescricao('');
    setFinalidade(EnumFinalidade.Ambas);
    setErro('');
  };

  return (
    <div className="categorias-container">
      <h2>📂 Gerenciar Categorias</h2>

      {/* Formulário para criar/editar categoria */}
      <form onSubmit={handleSubmit} className="categoria-form">
        <div className="form-group">
          <label htmlFor="descricao">Descrição:</label>
          <input
            id="descricao"
            type="text"
            value={descricao}
            onChange={(e) => setDescricao(e.target.value)}
            placeholder="Digite a descrição da categoria"
            maxLength={400}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="finalidade">Finalidade:</label>
          <select
            id="finalidade"
            value={finalidade}
            onChange={(e) => setFinalidade(parseInt(e.target.value) as EnumFinalidade)}
          >
            <option value={EnumFinalidade.Despesa}>Despesa</option>
            <option value={EnumFinalidade.Receita}>Receita</option>
            <option value={EnumFinalidade.Ambas}>Ambas</option>
          </select>
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

      {/* Lista de categorias */}
      <div className="categorias-lista">
        {loading ? (
          <p className="loading">Carregando...</p>
        ) : categorias.length === 0 ? (
          <p className="empty">Nenhuma categoria cadastrada</p>
        ) : (
          <table className="tabela-categorias">
            <thead>
              <tr>
                <th>ID</th>
                <th>Descrição</th>
                <th>Finalidade</th>
                <th>Data Criação</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {categorias.map((categoria) => (
                <tr key={categoria.id}>
                  <td>{categoria.id}</td>
                  <td>{categoria.descricao}</td>
                  <td>
                    <span className={`finalidade finalidade-${FINALIDADE_LABELS[categoria.finalidade].toLowerCase()}`}>
                      {FINALIDADE_LABELS[categoria.finalidade]}
                    </span>
                  </td>
                  <td>{new Date(categoria.dataCriacao).toLocaleDateString('pt-BR')}</td>
                  <td className="acoes">
                    <button onClick={() => handleEdit(categoria)} className="btn-edit" title="Editar">
                      ✎
                    </button>
                    <button onClick={() => handleDelete(categoria.id)} className="btn-delete" title="Deletar">
                      🗑️
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <p className="total">Total de categorias: {categorias.length}</p>
    </div>
  );
};

export default CategoriasComponent;
