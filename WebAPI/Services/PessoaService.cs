using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Data;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.Services
{
    /// <summary>
    /// Serviço para gerenciar operações com Pessoas
    /// Implementa lógica de negócio para CRUD e validações
    /// </summary>
    public interface IPessoaService
    {
        /// <summary>Cria uma nova pessoa</summary>
        Task<PessoaDTO> CriarPessoaAsync(CriarPessoaDTO dto);

        /// <summary>Obtém uma pessoa pelo ID</summary>
        Task<PessoaDTO?> ObterPessoaPorIdAsync(int id);

        /// <summary>Lista todas as pessoas cadastradas</summary>
        Task<List<PessoaDTO>> ListarPessoasAsync();

        /// <summary>Atualiza uma pessoa existente</summary>
        Task<PessoaDTO> AtualizarPessoaAsync(int id, CriarPessoaDTO dto);

        /// <summary>
        /// Deleta uma pessoa e todas as suas transações associadas
        /// Implementa exclusão em cascata
        /// </summary>
        Task DeletarPessoaAsync(int id);
    }

    /// <summary>
    /// Implementação do serviço de Pessoa
    /// </summary>
    public class PessoaService : IPessoaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PessoaService> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public PessoaService(ApplicationDbContext context, ILogger<PessoaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova pessoa no banco de dados
        /// Valida o nome e idade antes de inserir
        /// </summary>
        public async Task<PessoaDTO> CriarPessoaAsync(CriarPessoaDTO dto)
        {
            // Validação: Nome não pode estar vazio
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome da pessoa é obrigatório");

            // Validação: Nome não pode exceder 200 caracteres
            if (dto.Nome.Length > 200)
                throw new ArgumentException("Nome não pode ter mais de 200 caracteres");

            // Validação: Idade deve ser positiva
            if (dto.Idade < 0)
                throw new ArgumentException("Idade deve ser um valor positivo");

            var pessoa = new Pessoa
            {
                Nome = dto.Nome.Trim(),
                Idade = dto.Idade,
                DataCriacao = DateTime.UtcNow
            };

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Pessoa criada com sucesso: ID {pessoa.Id}, Nome: {pessoa.Nome}");

            return MapToPessoaDTO(pessoa);
        }

        /// <summary>Obtém uma pessoa pelo seu ID</summary>
        public async Task<PessoaDTO?> ObterPessoaPorIdAsync(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
            {
                _logger.LogWarning($"Pessoa com ID {id} não encontrada");
                return null;
            }

            return MapToPessoaDTO(pessoa);
        }

        /// <summary>Lista todas as pessoas cadastradas</summary>
        public async Task<List<PessoaDTO>> ListarPessoasAsync()
        {
            var pessoas = await _context.Pessoas
                .OrderBy(p => p.Nome)
                .ToListAsync();

            return pessoas.Select(MapToPessoaDTO).ToList();
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa existente
        /// Não permite atualizar o ID
        /// </summary>
        public async Task<PessoaDTO> AtualizarPessoaAsync(int id, CriarPessoaDTO dto)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);

            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada");

            // Validações
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome da pessoa é obrigatório");

            if (dto.Nome.Length > 200)
                throw new ArgumentException("Nome não pode ter mais de 200 caracteres");

            if (dto.Idade < 0)
                throw new ArgumentException("Idade deve ser um valor positivo");

            // Atualiza os dados
            pessoa.Nome = dto.Nome.Trim();
            pessoa.Idade = dto.Idade;

            _context.Pessoas.Update(pessoa);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Pessoa atualizada com sucesso: ID {id}");

            return MapToPessoaDTO(pessoa);
        }

        /// <summary>
        /// Deleta uma pessoa e todas as suas transações associadas
        /// A exclusão em cascata é configurada no DbContext
        /// </summary>
        public async Task DeletarPessoaAsync(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Transacoes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pessoa == null)
                throw new KeyNotFoundException($"Pessoa com ID {id} não encontrada");

            // Log das transações que serão deletadas
            var qtdTransacoes = pessoa.Transacoes.Count;

            _context.Pessoas.Remove(pessoa);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                $"Pessoa deletada com sucesso: ID {id}, Nome: {pessoa.Nome}. " +
                $"{qtdTransacoes} transação(ões) também foi/foram deletada(s) em cascata");
        }

        /// <summary>
        /// Mapeia um objeto Pessoa para PessoaDTO
        /// Utilizado para retornar dados ao cliente
        /// </summary>
        private PessoaDTO MapToPessoaDTO(Pessoa pessoa) => new()
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade,
            DataCriacao = pessoa.DataCriacao
        };
    }
}
