using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Data;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.Services
{
    /// <summary>
    /// Serviço para gerenciar operações com Transações
    /// Implementa lógica de negócio, validações e consultas de totais
    /// </summary>
    public interface ITransacaoService
    {
        /// <summary>Cria uma nova transação com validações</summary>
        Task<TransacaoDTO> CriarTransacaoAsync(CriarTransacaoDTO dto);

        /// <summary>Obtém uma transação pelo ID</summary>
        Task<TransacaoDTO?> ObterTransacaoPorIdAsync(int id);

        /// <summary>Lista todas as transações cadastradas</summary>
        Task<List<TransacaoDTO>> ListarTransacoesAsync();

        /// <summary>Lista todas as transações de uma pessoa específica</summary>
        Task<List<TransacaoDTO>> ListarTransacoesPorPessoaAsync(int pessoaId);

        /// <summary>Atualiza uma transação existente com validações</summary>
        Task<TransacaoDTO> AtualizarTransacaoAsync(int id, CriarTransacaoDTO dto);

        /// <summary>Deleta uma transação existente</summary>
        Task DeletarTransacaoAsync(int id);

        /// <summary>Obtém o total de receitas, despesas e saldo por pessoa</summary>
        Task<List<ResumoPorPessoaDTO>> ObterResumoPorPessoaAsync();

        /// <summary>Obtém o total de receitas, despesas e saldo por categoria</summary>
        Task<List<ResumoPorCategoriaDTO>> ObterResumoPorCategoriaAsync();

        /// <summary>Obtém o total geral de todas as transações</summary>
        Task<TotalGeralDTO> ObterTotalGeralAsync();
    }

    /// <summary>
    /// Implementação do serviço de Transação
    /// </summary>
    public class TransacaoService : ITransacaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<TransacaoService> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public TransacaoService(
            ApplicationDbContext context,
            ICategoriaService categoriaService,
            ILogger<TransacaoService> logger)
        {
            _context = context;
            _categoriaService = categoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova transação com validações completas:
        /// - Pessoa deve existir
        /// - Categoria deve existir
        /// - Valor deve ser positivo
        /// - Se pessoa é menor de idade, apenas despesas são permitidas
        /// - Categoria deve ser compatível com o tipo de transação
        /// </summary>
        public async Task<TransacaoDTO> CriarTransacaoAsync(CriarTransacaoDTO dto)
        {
            // Validação: Pessoa deve existir
            var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {dto.PessoaId} não encontrada");

            // Validação: Categoria deve existir
            var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada");

            // Validação: Descrição não pode estar vazia
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição da transação é obrigatória");

            // Validação: Descrição não pode exceder 400 caracteres
            if (dto.Descricao.Length > 400)
                throw new ArgumentException("Descrição não pode ter mais de 400 caracteres");

            // Validação: Valor deve ser positivo
            if (dto.Valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            // Validação: Se pessoa é menor de idade, apenas despesas são permitidas
            if (pessoa.Idade < 18 && dto.Tipo == EnumTipoTransacao.Receita)
                throw new InvalidOperationException(
                    $"Menores de idade (pessoa com {pessoa.Idade} anos) não podem ter transações de receita. " +
                    $"Apenas despesas são permitidas.");

            // Validação: Categoria deve ser compatível com o tipo de transação
            var categoriaPodeSerUsada = await _categoriaService.CategoriaPodeSerUsadaAsync(dto.CategoriaId, dto.Tipo);
            if (!categoriaPodeSerUsada)
            {
                var tipoNome = dto.Tipo == EnumTipoTransacao.Despesa ? "Despesa" : "Receita";
                throw new InvalidOperationException(
                    $"A categoria '{categoria.Descricao}' não pode ser usada para {tipoNome}s. " +
                    $"Finalidade da categoria: {categoria.Finalidade}");
            }

            var transacao = new Transacao
            {
                Descricao = dto.Descricao.Trim(),
                Valor = dto.Valor,
                Tipo = dto.Tipo,
                CategoriaId = dto.CategoriaId,
                PessoaId = dto.PessoaId,
                DataCriacao = DateTime.UtcNow
            };

            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Transação criada com sucesso: ID {transacao.Id}, " +
                $"Tipo: {transacao.Tipo}, Valor: {transacao.Valor:C}, " +
                $"Pessoa: {pessoa.Nome}, Categoria: {categoria.Descricao}");

            return MapToTransacaoDTO(transacao, pessoa.Nome, categoria.Descricao);
        }

        /// <summary>Obtém uma transação pelo seu ID</summary>
        public async Task<TransacaoDTO?> ObterTransacaoPorIdAsync(int id)
        {
            var transacao = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transacao == null)
            {
                _logger.LogWarning($"Transação com ID {id} não encontrada");
                return null;
            }

            return MapToTransacaoDTO(transacao, transacao.Pessoa?.Nome ?? "", transacao.Categoria?.Descricao ?? "");
        }

        /// <summary>Lista todas as transações cadastradas</summary>
        public async Task<List<TransacaoDTO>> ListarTransacoesAsync()
        {
            var transacoes = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();

            return transacoes.Select(t =>
                MapToTransacaoDTO(t, t.Pessoa?.Nome ?? "", t.Categoria?.Descricao ?? "")
            ).ToList();
        }

        /// <summary>Lista todas as transações de uma pessoa específica</summary>
        public async Task<List<TransacaoDTO>> ListarTransacoesPorPessoaAsync(int pessoaId)
        {
            // Validação: Pessoa deve existir
            var pessoa = await _context.Pessoas.FindAsync(pessoaId);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {pessoaId} não encontrada");

            var transacoes = await _context.Transacoes
                .Where(t => t.PessoaId == pessoaId)
                .Include(t => t.Categoria)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();

            return transacoes.Select(t =>
                MapToTransacaoDTO(t, pessoa.Nome, t.Categoria?.Descricao ?? "")
            ).ToList();
        }

        /// <summary>Atualiza uma transação existente com as mesmas validações de criação</summary>
        public async Task<TransacaoDTO> AtualizarTransacaoAsync(int id, CriarTransacaoDTO dto)
        {
            var transacao = await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transacao == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada");

            // Validações (igual a CriarTransacaoAsync)
            var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);
            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {dto.PessoaId} não encontrada");

            var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {dto.CategoriaId} não encontrada");

            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição da transação é obrigatória");

            if (dto.Descricao.Length > 400)
                throw new ArgumentException("Descrição não pode ter mais de 400 caracteres");

            if (dto.Valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            if (pessoa.Idade < 18 && dto.Tipo == EnumTipoTransacao.Receita)
                throw new InvalidOperationException(
                    $"Menores de idade (pessoa com {pessoa.Idade} anos) não podem ter transações de receita");

            var categoriaPodeSerUsada = await _categoriaService.CategoriaPodeSerUsadaAsync(dto.CategoriaId, dto.Tipo);
            if (!categoriaPodeSerUsada)
                throw new InvalidOperationException(
                    $"A categoria não pode ser usada para este tipo de transação");

            // Atualiza os dados
            transacao.Descricao = dto.Descricao.Trim();
            transacao.Valor = dto.Valor;
            transacao.Tipo = dto.Tipo;
            transacao.CategoriaId = dto.CategoriaId;
            transacao.PessoaId = dto.PessoaId;

            _context.Transacoes.Update(transacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transação atualizada com sucesso: ID {id}");

            return MapToTransacaoDTO(transacao, pessoa.Nome, categoria.Descricao);
        }

        /// <summary>Deleta uma transação existente</summary>
        public async Task DeletarTransacaoAsync(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);

            if (transacao == null)
                throw new KeyNotFoundException($"Transação com ID {id} não encontrada");

            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transação deletada com sucesso: ID {id}");
        }

        /// <summary>
        /// Obtém um resumo de receitas, despesas e saldo para cada pessoa
        /// Agrupa todas as transações por pessoa e calcula os totais
        /// </summary>
        public async Task<List<ResumoPorPessoaDTO>> ObterResumoPorPessoaAsync()
        {
            var pessoas = await _context.Pessoas.ToListAsync();
            var resultado = new List<ResumoPorPessoaDTO>();

            foreach (var pessoa in pessoas)
            {
                var transacoes = await _context.Transacoes
                    .Where(t => t.PessoaId == pessoa.Id)
                    .ToListAsync();

                var receitas = transacoes
                    .Where(t => t.Tipo == EnumTipoTransacao.Receita)
                    .Sum(t => t.Valor);

                var despesas = transacoes
                    .Where(t => t.Tipo == EnumTipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                resultado.Add(new ResumoPorPessoaDTO
                {
                    PessoaId = pessoa.Id,
                    PessoaNome = pessoa.Nome,
                    Idade = pessoa.Idade,
                    TotalReceitas = receitas,
                    TotalDespesas = despesas,
                    Saldo = receitas - despesas
                });
            }

            return resultado.OrderBy(r => r.PessoaNome).ToList();
        }

        /// <summary>
        /// Obtém um resumo de receitas, despesas e saldo para cada categoria
        /// Agrupa todas as transações por categoria e calcula os totais
        /// </summary>
        public async Task<List<ResumoPorCategoriaDTO>> ObterResumoPorCategoriaAsync()
        {
            var categorias = await _context.Categorias.ToListAsync();
            var resultado = new List<ResumoPorCategoriaDTO>();

            foreach (var categoria in categorias)
            {
                var transacoes = await _context.Transacoes
                    .Where(t => t.CategoriaId == categoria.Id)
                    .ToListAsync();

                var receitas = transacoes
                    .Where(t => t.Tipo == EnumTipoTransacao.Receita)
                    .Sum(t => t.Valor);

                var despesas = transacoes
                    .Where(t => t.Tipo == EnumTipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                resultado.Add(new ResumoPorCategoriaDTO
                {
                    CategoriaId = categoria.Id,
                    CategoriaDescricao = categoria.Descricao,
                    Finalidade = categoria.Finalidade,
                    TotalReceitas = receitas,
                    TotalDespesas = despesas,
                    Saldo = receitas - despesas
                });
            }

            return resultado.OrderBy(r => r.CategoriaDescricao).ToList();
        }

        /// <summary>
        /// Obtém o total geral de todas as transações
        /// Calcula receitas, despesas e saldo consolidados
        /// </summary>
        public async Task<TotalGeralDTO> ObterTotalGeralAsync()
        {
            var transacoes = await _context.Transacoes.ToListAsync();

            var totalReceitas = transacoes
                .Where(t => t.Tipo == EnumTipoTransacao.Receita)
                .Sum(t => t.Valor);

            var totalDespesas = transacoes
                .Where(t => t.Tipo == EnumTipoTransacao.Despesa)
                .Sum(t => t.Valor);

            return new TotalGeralDTO
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                SaldoLiquido = totalReceitas - totalDespesas,
                QtdTransacoes = transacoes.Count,
                QtdPessoas = await _context.Pessoas.CountAsync(),
                QtdCategorias = await _context.Categorias.CountAsync()
            };
        }

        /// <summary>
        /// Mapeia um objeto Transacao para TransacaoDTO
        /// Utilizado para retornar dados ao cliente
        /// </summary>
        private TransacaoDTO MapToTransacaoDTO(Transacao transacao, string pessoaNome, string categoriaDescricao) => new()
        {
            Id = transacao.Id,
            Descricao = transacao.Descricao,
            Valor = transacao.Valor,
            Tipo = transacao.Tipo,
            CategoriaId = transacao.CategoriaId,
            CategoriaDescricao = categoriaDescricao,
            PessoaId = transacao.PessoaId,
            PessoaNome = pessoaNome,
            DataCriacao = transacao.DataCriacao
        };
    }
}
