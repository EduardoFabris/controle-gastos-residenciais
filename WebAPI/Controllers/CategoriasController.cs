using Microsoft.AspNetCore.Mvc;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Services;

namespace ControleGastosResidenciais.Controllers
{
    /// <summary>
    /// Controller para operações de CRUD de Categorias
    /// Gerencia criação, leitura, atualização e deleção de categorias de transações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova categoria
        /// </summary>
        /// <param name="dto">Dados da categoria a ser criada</param>
        /// <returns>Categoria criada com seu ID</returns>
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> CriarCategoria([FromBody] CriarCategoriaDTO dto)
        {
            try
            {
                var resultado = await _categoriaService.CriarCategoriaAsync(dto);
                return CreatedAtAction(nameof(ObterCategoria), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao criar categoria: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma categoria específica pelo ID
        /// </summary>
        /// <param name="id">ID da categoria a buscar</param>
        /// <returns>Dados da categoria se encontrada</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> ObterCategoria(int id)
        {
            var categoria = await _categoriaService.ObterCategoriaPorIdAsync(id);

            if (categoria == null)
                return NotFound(new { erro = $"Categoria com ID {id} não encontrada" });

            return Ok(categoria);
        }

        /// <summary>
        /// Lista todas as categorias cadastradas
        /// </summary>
        /// <returns>Lista de todas as categorias ordenadas por descrição</returns>
        [HttpGet]
        public async Task<ActionResult<List<CategoriaDTO>>> ListarCategorias()
        {
            var categorias = await _categoriaService.ListarCategoriasAsync();
            return Ok(categorias);
        }

        /// <summary>
        /// Atualiza os dados de uma categoria existente
        /// </summary>
        /// <param name="id">ID da categoria a atualizar</param>
        /// <param name="dto">Novos dados da categoria</param>
        /// <returns>Categoria atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoriaDTO>> AtualizarCategoria(int id, [FromBody] CriarCategoriaDTO dto)
        {
            try
            {
                var resultado = await _categoriaService.AtualizarCategoriaAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Categoria não encontrada: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao atualizar categoria: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma categoria existente
        /// Não permite deletar categorias que possuem transações associadas
        /// </summary>
        /// <param name="id">ID da categoria a deletar</param>
        /// <returns>Confirmação de deleção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarCategoria(int id)
        {
            try
            {
                await _categoriaService.DeletarCategoriaAsync(id);
                return Ok(new { mensagem = $"Categoria com ID {id} deletada com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Categoria não encontrada: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Não foi possível deletar categoria: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}
