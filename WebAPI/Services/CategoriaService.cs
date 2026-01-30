using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Data;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.Services
{
    /// <summary>
    /// Serviço para gerenciar operações com Categorias
    /// Implementa lógica de negócio para CRUD e validações
    /// </summary>
    public interface ICategoriaService
    {
        /// <summary>Cria uma nova categoria</summary>
        Task<CategoriaDTO> CriarCategoriaAsync(CriarCategoriaDTO dto);

        /// <summary>Obtém uma categoria pelo ID</summary>
        Task<CategoriaDTO?> ObterCategoriaPorIdAsync(int id);

        /// <summary>Lista todas as categorias cadastradas</summary>
        Task<List<CategoriaDTO>> ListarCategoriasAsync();

        /// <summary>Atualiza uma categoria existente</summary>
        Task<CategoriaDTO> AtualizarCategoriaAsync(int id, CriarCategoriaDTO dto);

        /// <summary>Deleta uma categoria existente</summary>
        Task DeletarCategoriaAsync(int id);

        /// <summary>Verifica se uma categoria pode ser usada para um tipo específico de transação</summary>
        Task<bool> CategoriaPodeSerUsadaAsync(int categoriaId, EnumTipoTransacao tipo);
    }

    /// <summary>
    /// Implementação do serviço de Categoria
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoriaService> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public CategoriaService(ApplicationDbContext context, ILogger<CategoriaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova categoria no banco de dados
        /// Valida a descrição e finalidade antes de inserir
        /// </summary>
        public async Task<CategoriaDTO> CriarCategoriaAsync(CriarCategoriaDTO dto)
        {
            // Validação: Descrição não pode estar vazia
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição da categoria é obrigatória");

            // Validação: Descrição não pode exceder 400 caracteres
            if (dto.Descricao.Length > 400)
                throw new ArgumentException("Descrição não pode ter mais de 400 caracteres");

            // Validação: Finalidade deve ser válida (1=Despesa, 2=Receita, 3=Ambas)
            if (!Enum.IsDefined(typeof(EnumFinalidade), dto.Finalidade))
                throw new ArgumentException("Finalidade inválida. Use: 1=Despesa, 2=Receita, 3=Ambas");

            var categoria = new Categoria
            {
                Descricao = dto.Descricao.Trim(),
                Finalidade = dto.Finalidade,
                DataCriacao = DateTime.UtcNow
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Categoria criada com sucesso: ID {categoria.Id}, Descrição: {categoria.Descricao}");

            return MapToCategoriaDTO(categoria);
        }

        /// <summary>Obtém uma categoria pelo seu ID</summary>
        public async Task<CategoriaDTO?> ObterCategoriaPorIdAsync(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                _logger.LogWarning($"Categoria com ID {id} não encontrada");
                return null;
            }

            return MapToCategoriaDTO(categoria);
        }

        /// <summary>Lista todas as categorias cadastradas</summary>
        public async Task<List<CategoriaDTO>> ListarCategoriasAsync()
        {
            var categorias = await _context.Categorias
                .OrderBy(c => c.Descricao)
                .ToListAsync();

            return categorias.Select(MapToCategoriaDTO).ToList();
        }

        /// <summary>
        /// Atualiza os dados de uma categoria existente
        /// Não permite atualizar o ID
        /// </summary>
        public async Task<CategoriaDTO> AtualizarCategoriaAsync(int id, CriarCategoriaDTO dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");

            // Validações
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new ArgumentException("Descrição da categoria é obrigatória");

            if (dto.Descricao.Length > 400)
                throw new ArgumentException("Descrição não pode ter mais de 400 caracteres");

            if (!Enum.IsDefined(typeof(EnumFinalidade), dto.Finalidade))
                throw new ArgumentException("Finalidade inválida. Use: 1=Despesa, 2=Receita, 3=Ambas");

            // Atualiza os dados
            categoria.Descricao = dto.Descricao.Trim();
            categoria.Finalidade = dto.Finalidade;

            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Categoria atualizada com sucesso: ID {id}");

            return MapToCategoriaDTO(categoria);
        }

        /// <summary>
        /// Deleta uma categoria existente
        /// Não permite deletar categorias que possuem transações associadas
        /// </summary>
        public async Task DeletarCategoriaAsync(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                throw new KeyNotFoundException($"Categoria com ID {id} não encontrada");

            // Validação: Não permite deletar categorias com transações
            if (categoria.Transacoes.Count > 0)
                throw new InvalidOperationException(
                    $"Não é possível deletar a categoria '{categoria.Descricao}' " +
                    $"porque ela possui {categoria.Transacoes.Count} transação(ões) associada(s)");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Categoria deletada com sucesso: ID {id}, Descrição: {categoria.Descricao}");
        }

        /// <summary>
        /// Verifica se uma categoria pode ser usada para um tipo específico de transação
        /// Valida se a finalidade da categoria é compatível com o tipo
        /// </summary>
        public async Task<bool> CategoriaPodeSerUsadaAsync(int categoriaId, EnumTipoTransacao tipo)
        {
            var categoria = await _context.Categorias.FindAsync(categoriaId);

            if (categoria == null)
                return false;

            // Se a categoria aceita ambas, sempre retorna true
            if (categoria.Finalidade == EnumFinalidade.Ambas)
                return true;

            // Se é despesa, a categoria deve aceitar despesas
            if (tipo == EnumTipoTransacao.Despesa)
                return categoria.Finalidade == EnumFinalidade.Despesa;

            // Se é receita, a categoria deve aceitar receitas
            if (tipo == EnumTipoTransacao.Receita)
                return categoria.Finalidade == EnumFinalidade.Receita;

            return false;
        }

        /// <summary>
        /// Mapeia um objeto Categoria para CategoriaDTO
        /// Utilizado para retornar dados ao cliente
        /// </summary>
        private CategoriaDTO MapToCategoriaDTO(Categoria categoria) => new()
        {
            Id = categoria.Id,
            Descricao = categoria.Descricao,
            Finalidade = categoria.Finalidade,
            DataCriacao = categoria.DataCriacao
        };
    }
}
