/**
 * Serviço API para comunicação com o backend
 * Centraliza todas as requisições HTTP para a WebAPI
 */

import axios, { AxiosInstance } from 'axios';

// Configuração do cliente Axios
const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

const apiClient: AxiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// ==========================================
// TIPOS E INTERFACES
// ==========================================

export interface Pessoa {
  id: number;
  nome: string;
  idade: number;
  dataCriacao: string;
}

export interface Categoria {
  id: number;
  descricao: string;
  finalidade: EnumFinalidade;
  dataCriacao: string;
}

export interface Transacao {
  id: number;
  descricao: string;
  valor: number;
  tipo: EnumTipoTransacao;
  categoriaId: number;
  categoriaDescricao: string;
  pessoaId: number;
  pessoaNome: string;
  dataCriacao: string;
}

export interface ResumoPorPessoa {
  pessoaId: number;
  pessoaNome: string;
  idade: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface ResumoPorCategoria {
  categoriaId: number;
  categoriaDescricao: string;
  finalidade: EnumFinalidade;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface TotalGeral {
  totalReceitas: number;
  totalDespesas: number;
  saldoLiquido: number;
  qtdTransacoes: number;
  qtdPessoas: number;
  qtdCategorias: number;
}

export enum EnumFinalidade {
  Despesa = 1,
  Receita = 2,
  Ambas = 3,
}

export enum EnumTipoTransacao {
  Despesa = 1,
  Receita = 2,
}

// ==========================================
// SERVIÇOS - PESSOAS
// ==========================================

export const pessoaService = {
  /**
   * Cria uma nova pessoa
   */
  criarPessoa: async (nome: string, idade: number): Promise<Pessoa> => {
    const response = await apiClient.post('/pessoas', { nome, idade });
    return response.data;
  },

  /**
   * Lista todas as pessoas cadastradas
   */
  listarPessoas: async (): Promise<Pessoa[]> => {
    const response = await apiClient.get('/pessoas');
    return response.data;
  },

  /**
   * Obtém uma pessoa pelo ID
   */
  obterPessoaById: async (id: number): Promise<Pessoa | null> => {
    try {
      const response = await apiClient.get(`/pessoas/${id}`);
      return response.data;
    } catch (error) {
      return null;
    }
  },

  /**
   * Atualiza uma pessoa existente
   */
  atualizarPessoa: async (id: number, nome: string, idade: number): Promise<Pessoa> => {
    const response = await apiClient.put(`/pessoas/${id}`, { nome, idade });
    return response.data;
  },

  /**
   * Deleta uma pessoa (e suas transações em cascata)
   */
  deletarPessoa: async (id: number): Promise<void> => {
    await apiClient.delete(`/pessoas/${id}`);
  },
};

// ==========================================
// SERVIÇOS - CATEGORIAS
// ==========================================

export const categoriaService = {
  /**
   * Cria uma nova categoria
   */
  criarCategoria: async (descricao: string, finalidade: EnumFinalidade): Promise<Categoria> => {
    const response = await apiClient.post('/categorias', { descricao, finalidade });
    return response.data;
  },

  /**
   * Lista todas as categorias cadastradas
   */
  listarCategorias: async (): Promise<Categoria[]> => {
    const response = await apiClient.get('/categorias');
    return response.data;
  },

  /**
   * Obtém uma categoria pelo ID
   */
  obterCategoriaById: async (id: number): Promise<Categoria | null> => {
    try {
      const response = await apiClient.get(`/categorias/${id}`);
      return response.data;
    } catch (error) {
      return null;
    }
  },

  /**
   * Atualiza uma categoria existente
   */
  atualizarCategoria: async (id: number, descricao: string, finalidade: EnumFinalidade): Promise<Categoria> => {
    const response = await apiClient.put(`/categorias/${id}`, { descricao, finalidade });
    return response.data;
  },

  /**
   * Deleta uma categoria
   */
  deletarCategoria: async (id: number): Promise<void> => {
    await apiClient.delete(`/categorias/${id}`);
  },
};

// ==========================================
// SERVIÇOS - TRANSAÇÕES
// ==========================================

export const transacaoService = {
  /**
   * Cria uma nova transação
   */
  criarTransacao: async (
    descricao: string,
    valor: number,
    tipo: EnumTipoTransacao,
    categoriaId: number,
    pessoaId: number
  ): Promise<Transacao> => {
    const response = await apiClient.post('/transacoes', {
      descricao,
      valor,
      tipo,
      categoriaId,
      pessoaId,
    });
    return response.data;
  },

  /**
   * Lista todas as transações cadastradas
   */
  listarTransacoes: async (): Promise<Transacao[]> => {
    const response = await apiClient.get('/transacoes');
    return response.data;
  },

  /**
   * Obtém uma transação pelo ID
   */
  obterTransacaoById: async (id: number): Promise<Transacao | null> => {
    try {
      const response = await apiClient.get(`/transacoes/${id}`);
      return response.data;
    } catch (error) {
      return null;
    }
  },

  /**
   * Lista as transações de uma pessoa específica
   */
  listarTransacoesPorPessoa: async (pessoaId: number): Promise<Transacao[]> => {
    const response = await apiClient.get(`/transacoes/pessoa/${pessoaId}`);
    return response.data;
  },

  /**
   * Atualiza uma transação existente
   */
  atualizarTransacao: async (
    id: number,
    descricao: string,
    valor: number,
    tipo: EnumTipoTransacao,
    categoriaId: number,
    pessoaId: number
  ): Promise<Transacao> => {
    const response = await apiClient.put(`/transacoes/${id}`, {
      descricao,
      valor,
      tipo,
      categoriaId,
      pessoaId,
    });
    return response.data;
  },

  /**
   * Deleta uma transação
   */
  deletarTransacao: async (id: number): Promise<void> => {
    await apiClient.delete(`/transacoes/${id}`);
  },

  /**
   * Obtém resumo por pessoa com totais consolidados
   */
  obterResumoPorPessoa: async (): Promise<{ pessoas: ResumoPorPessoa[]; totalGeral: TotalGeral }> => {
    const response = await apiClient.get('/transacoes/resumo/pessoa');
    return response.data;
  },

  /**
   * Obtém resumo por categoria com totais consolidados
   */
  obterResumoPorCategoria: async (): Promise<{ categorias: ResumoPorCategoria[]; totalGeral: TotalGeral }> => {
    const response = await apiClient.get('/transacoes/resumo/categoria');
    return response.data;
  },

  /**
   * Obtém total geral consolidado
   */
  obterTotalGeral: async (): Promise<TotalGeral> => {
    const response = await apiClient.get('/transacoes/resumo/total');
    return response.data;
  },
};

export default apiClient;
