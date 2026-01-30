import React, { useState, useEffect } from 'react';
import {
  Transacao,
  transacaoService,
  pessoaService,
  categoriaService,
  Pessoa,
  Categoria,
  EnumTipoTransacao,
} from '../services/apiService';
import './Transacoes.css';

/**
 * Componente para gerenciar transações
 * CRUD completo com validações: menores de idade só despesas, categoria compatível com tipo
 */
export const TransacoesComponent: React.FC = () => {
  const [transacoes, setTransacoes] = useState<Transacao[]>([]);
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);

  // Form fields
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState('');
  const [tipo, setTipo] = useState<EnumTipoTransacao>(EnumTipoTransacao.Despesa);
  const [categoriaId, setCategoriaId] = useState<number>(0);
  const [pessoaId, setPessoaId] = useState<number>(0);

  const [editingId, setEditingId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [erro, setErro] = useState('');
  const [filtroIdade, setFiltroIdade] = useState(false);

  // Carrega dados ao montar o componente
  useEffect(() => {
    carregarTransacoes();
    carregarPessoas();
    carregarCategorias();
  }, []);

  /**
   * Carrega todas as transações do backend
   */
  const carregarTransacoes = async () => {
    try {
      setLoading(true);
      setErro('');
      const dados = await transacaoService.listarTransacoes();
      setTransacoes(dados);
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao carregar transações');
    } finally {
      setLoading(false);
    }
  };

  /**
   * Carrega todas as pessoas
   */
  const carregarPessoas = async () => {
    try {
      const dados = await pessoaService.listarPessoas();
      setPessoas(dados);
    } catch (error) {
      console.error('Erro ao carregar pessoas:', error);
    }
  };

  /**
   * Carrega todas as categorias
   */
  const carregarCategorias = async () => {
    try {
      const dados = await categoriaService.listarCategorias();
      setCategorias(dados);
    } catch (error) {
      console.error('Erro ao carregar categorias:', error);
    }
  };

  /**
   * Filtra categorias compatíveis com o tipo selecionado
   */
  const categoriasCompativeis = categorias.filter((cat) => {
    // Se a categoria é "Ambas", sempre aparece
    if (cat.finalidade === 3) return true;
    // Se o tipo é despesa, categoria deve aceitar despesa
    if (tipo === EnumTipoTransacao.Despesa && cat.finalidade === 1) return true;
    // Se o tipo é receita, categoria deve aceitar receita
    if (tipo === EnumTipoTransacao.Receita && cat.finalidade === 2) return true;
    return false;
  });

  /**
   * Cria ou atualiza uma transação
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setErro('');
      const valorNum = parseFloat(valor);

      // Validação: pessoa deve existir
      if (!pessoaId) {
        setErro('Selecione uma pessoa');
        return;
      }

      // Validação: categoria deve existir
      if (!categoriaId) {
        setErro('Selecione uma categoria');
        return;
      }

      if (editingId) {
        // Atualizar transação existente
        const transacao = await transacaoService.atualizarTransacao(
          editingId,
          descricao,
          valorNum,
          tipo,
          categoriaId,
          pessoaId
        );
        setTransacoes(transacoes.map(t => (t.id === editingId ? transacao : t)));
        setEditingId(null);
      } else {
        // Criar nova transação
        const transacao = await transacaoService.criarTransacao(
          descricao,
          valorNum,
          tipo,
          categoriaId,
          pessoaId
        );
        setTransacoes([...transacoes, transacao]);
      }

      // Limpa o formulário
      setDescricao('');
      setValor('');
      setTipo(EnumTipoTransacao.Despesa);
      setCategoriaId(0);
      setPessoaId(0);
    } catch (error: any) {
      setErro(error.response?.data?.erro || 'Erro ao salvar transação');
    }
  };

  /**
   * Inicia a edição de uma transação
   */
  const handleEdit = (transacao: Transacao) => {
    setEditingId(transacao.id);
    setDescricao(transacao.descricao);
    setValor(transacao.valor.toString());
    setTipo(transacao.tipo);
    setCategoriaId(transacao.categoriaId);
    setPessoaId(transacao.pessoaId);
  };

  /**
   * Deleta uma transação
   */
  const handleDelete = async (id: number) => {
    if (window.confirm('Tem certeza que deseja deletar esta transação?')) {
      try {
        setErro('');
        await transacaoService.deletarTransacao(id);
        setTransacoes(transacoes.filter(t => t.id !== id));
      } catch (error: any) {
        setErro(error.response?.data?.erro || 'Erro ao deletar transação');
      }
    }
  };

  /**
   * Cancela a edição
   */
  const handleCancel = () => {
    setEditingId(null);
    setDescricao('');
    setValor('');
    setTipo(EnumTipoTransacao.Despesa);
    setCategoriaId(0);
    setPessoaId(0);
    setErro('');
  };

  /**
   * Formata valor monetário
   */
  const formatarMoeda = (valor: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(valor);
  };

  // Obter dados da pessoa selecionada para validação
  const pessoaSelecionada = pessoas.find(p => p.id === pessoaId);

  return (
    <div className="transacoes-container">
      <h2>💰 Gerenciar Transações</h2>

      {/* Formulário para criar/editar transação */}
      <form onSubmit={handleSubmit} className="transacao-form">
        <div className="form-row">
          <div className="form-group">
            <label htmlFor="descricao">Descrição:</label>
            <input
              id="descricao"
              type="text"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              placeholder="Digite a descrição da transação"
              maxLength={400}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="valor">Valor:</label>
            <input
              id="valor"
              type="number"
              step="0.01"
              value={valor}
              onChange={(e) => setValor(e.target.value)}
              placeholder="0.00"
              min="0.01"
              required
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-group">
            <label htmlFor="tipo">Tipo:</label>
            <select
              id="tipo"
              value={tipo}
              onChange={(e) => {
                setTipo(parseInt(e.target.value) as EnumTipoTransacao);
                setCategoriaId(0); // Reseta categoria ao mudar tipo
              }}
            >
              <option value={EnumTipoTransacao.Despesa}>Despesa</option>
              <option value={EnumTipoTransacao.Receita}>Receita</option>
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="pessoa">Pessoa:</label>
            <select
              id="pessoa"
              value={pessoaId}
              onChange={(e) => setPessoaId(parseInt(e.target.value))}
              required
            >
              <option value={0}>-- Selecione uma pessoa --</option>
              {pessoas.map((pessoa) => (
                <option key={pessoa.id} value={pessoa.id}>
                  {pessoa.nome} ({pessoa.idade} anos)
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="categoria">Categoria:</label>
            <select
              id="categoria"
              value={categoriaId}
              onChange={(e) => setCategoriaId(parseInt(e.target.value))}
              required
            >
              <option value={0}>-- Selecione uma categoria --</option>
              {categoriasCompativeis.map((categoria) => (
                <option key={categoria.id} value={categoria.id}>
                  {categoria.descricao}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Aviso para menores de idade */}
        {pessoaSelecionada && pessoaSelecionada.idade < 18 && tipo === EnumTipoTransacao.Receita && (
          <div className="alert alert-warning">
            ⚠️ Aviso: A pessoa selecionada tem {pessoaSelecionada.idade} anos. Menores de idade só podem ter transações de despesa!
          </div>
        )}

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

      {/* Filtros */}
      <div className="filtros">
        <label className="checkbox">
          <input
            type="checkbox"
            checked={filtroIdade}
            onChange={(e) => setFiltroIdade(e.target.checked)}
          />
          Mostrar apenas despesas de menores de idade
        </label>
      </div>

      {/* Lista de transações */}
      <div className="transacoes-lista">
        {loading ? (
          <p className="loading">Carregando...</p>
        ) : transacoes.length === 0 ? (
          <p className="empty">Nenhuma transação cadastrada</p>
        ) : (
          <table className="tabela-transacoes">
            <thead>
              <tr>
                <th>ID</th>
                <th>Descrição</th>
                <th>Valor</th>
                <th>Tipo</th>
                <th>Pessoa</th>
                <th>Categoria</th>
                <th>Data</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {transacoes
                .filter((transacao) => {
                  if (filtroIdade) {
                    const pessoa = pessoas.find(p => p.id === transacao.pessoaId);
                    return pessoa && pessoa.idade < 18 && transacao.tipo === EnumTipoTransacao.Despesa;
                  }
                  return true;
                })
                .map((transacao) => (
                  <tr key={transacao.id}>
                    <td>{transacao.id}</td>
                    <td>{transacao.descricao}</td>
                    <td className={transacao.tipo === EnumTipoTransacao.Receita ? 'valor-receita' : 'valor-despesa'}>
                      {transacao.tipo === EnumTipoTransacao.Receita ? '+' : '-'} {formatarMoeda(transacao.valor)}
                    </td>
                    <td>
                      <span className={`tipo tipo-${transacao.tipo === EnumTipoTransacao.Despesa ? 'despesa' : 'receita'}`}>
                        {transacao.tipo === EnumTipoTransacao.Despesa ? 'Despesa' : 'Receita'}
                      </span>
                    </td>
                    <td>{transacao.pessoaNome}</td>
                    <td>{transacao.categoriaDescricao}</td>
                    <td>{new Date(transacao.dataCriacao).toLocaleDateString('pt-BR')}</td>
                    <td className="acoes">
                      <button onClick={() => handleEdit(transacao)} className="btn-edit" title="Editar">
                        ✎
                      </button>
                      <button onClick={() => handleDelete(transacao.id)} className="btn-delete" title="Deletar">
                        🗑️
                      </button>
                    </td>
                  </tr>
                ))}
            </tbody>
          </table>
        )}
      </div>

      <p className="total">Total de transações: {transacoes.length}</p>
    </div>
  );
};

export default TransacoesComponent;
