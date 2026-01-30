using Microsoft.AspNetCore.Mvc;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Services;

namespace ControleGastosResidenciais.Controllers
{
    /// <summary>
    /// Controller para operações de CRUD de Pessoas (Residentes)
    /// Gerencia criação, leitura, atualização e deleção de pessoas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly ILogger<PessoasController> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public PessoasController(IPessoaService pessoaService, ILogger<PessoasController> logger)
        {
            _pessoaService = pessoaService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova pessoa
        /// </summary>
        /// <param name="dto">Dados da pessoa a ser criada</param>
        /// <returns>Pessoa criada com seu ID</returns>
        [HttpPost]
        public async Task<ActionResult<PessoaDTO>> CriarPessoa([FromBody] CriarPessoaDTO dto)
        {
            try
            {
                var resultado = await _pessoaService.CriarPessoaAsync(dto);
                return CreatedAtAction(nameof(ObterPessoa), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao criar pessoa: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma pessoa específica pelo ID
        /// </summary>
        /// <param name="id">ID da pessoa a buscar</param>
        /// <returns>Dados da pessoa se encontrada</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaDTO>> ObterPessoa(int id)
        {
            var pessoa = await _pessoaService.ObterPessoaPorIdAsync(id);

            if (pessoa == null)
                return NotFound(new { erro = $"Pessoa com ID {id} não encontrada" });

            return Ok(pessoa);
        }

        /// <summary>
        /// Lista todas as pessoas cadastradas
        /// </summary>
        /// <returns>Lista de todas as pessoas ordenadas por nome</returns>
        [HttpGet]
        public async Task<ActionResult<List<PessoaDTO>>> ListarPessoas()
        {
            var pessoas = await _pessoaService.ListarPessoasAsync();
            return Ok(pessoas);
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa existente
        /// </summary>
        /// <param name="id">ID da pessoa a atualizar</param>
        /// <param name="dto">Novos dados da pessoa</param>
        /// <returns>Pessoa atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<PessoaDTO>> AtualizarPessoa(int id, [FromBody] CriarPessoaDTO dto)
        {
            try
            {
                var resultado = await _pessoaService.AtualizarPessoaAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Pessoa não encontrada: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao atualizar pessoa: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma pessoa e todas as suas transações associadas (exclusão em cascata)
        /// </summary>
        /// <param name="id">ID da pessoa a deletar</param>
        /// <returns>Confirmação de deleção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarPessoa(int id)
        {
            try
            {
                await _pessoaService.DeletarPessoaAsync(id);
                return Ok(new { mensagem = $"Pessoa com ID {id} deletada com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Pessoa não encontrada: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
        }
    }
}
