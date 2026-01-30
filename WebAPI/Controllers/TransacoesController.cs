using Microsoft.AspNetCore.Mvc;
using ControleGastosResidenciais.DTOs;
using ControleGastosResidenciais.Services;

namespace ControleGastosResidenciais.Controllers
{
    /// <summary>
    /// Controller para operações de CRUD de Transações e Consultas de Totais
    /// Gerencia criação, leitura, atualização e deleção de transações
    /// Fornece endpoints para consultas consolidadas de receitas, despesas e saldos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ILogger<TransacoesController> _logger;

        /// <summary>Injeta as dependências necessárias</summary>
        public TransacoesController(ITransacaoService transacaoService, ILogger<TransacoesController> logger)
        {
            _transacaoService = transacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma nova transação com validações completas
        /// Valida: pessoa existe, categoria existe, categoria é compatível com o tipo, 
        /// menores de idade apenas podem ter despesas
        /// </summary>
        /// <param name="dto">Dados da transação a ser criada</param>
        /// <returns>Transação criada com seu ID</returns>
        [HttpPost]
        public async Task<ActionResult<TransacaoDTO>> CriarTransacao([FromBody] CriarTransacaoDTO dto)
        {
            try
            {
                var resultado = await _transacaoService.CriarTransacaoAsync(dto);
                return CreatedAtAction(nameof(ObterTransacao), new { id = resultado.Id }, resultado);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Recurso não encontrado: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao criar transação: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operação inválida: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma transação específica pelo ID
        /// </summary>
        /// <param name="id">ID da transação a buscar</param>
        /// <returns>Dados da transação se encontrada</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransacaoDTO>> ObterTransacao(int id)
        {
            var transacao = await _transacaoService.ObterTransacaoPorIdAsync(id);

            if (transacao == null)
                return NotFound(new { erro = $"Transação com ID {id} não encontrada" });

            return Ok(transacao);
        }

        /// <summary>
        /// Lista todas as transações cadastradas em ordem decrescente de data de criação
        /// </summary>
        /// <returns>Lista de todas as transações</returns>
        [HttpGet]
        public async Task<ActionResult<List<TransacaoDTO>>> ListarTransacoes()
        {
            var transacoes = await _transacaoService.ListarTransacoesAsync();
            return Ok(transacoes);
        }

        /// <summary>
        /// Lista todas as transações de uma pessoa específica
        /// </summary>
        /// <param name="pessoaId">ID da pessoa para filtrar transações</param>
        /// <returns>Lista de transações da pessoa</returns>
        [HttpGet("pessoa/{pessoaId}")]
        public async Task<ActionResult<List<TransacaoDTO>>> ListarTransacoesPorPessoa(int pessoaId)
        {
            try
            {
                var transacoes = await _transacaoService.ListarTransacoesPorPessoaAsync(pessoaId);
                return Ok(transacoes);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza uma transação existente com as mesmas validações de criação
        /// </summary>
        /// <param name="id">ID da transação a atualizar</param>
        /// <param name="dto">Novos dados da transação</param>
        /// <returns>Transação atualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransacaoDTO>> AtualizarTransacao(int id, [FromBody] CriarTransacaoDTO dto)
        {
            try
            {
                var resultado = await _transacaoService.AtualizarTransacaoAsync(id, dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Recurso não encontrado: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Validação falhou ao atualizar transação: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Operação inválida: {ex.Message}");
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma transação existente
        /// </summary>
        /// <param name="id">ID da transação a deletar</param>
        /// <returns>Confirmação de deleção</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarTransacao(int id)
        {
            try
            {
                await _transacaoService.DeletarTransacaoAsync(id);
                return Ok(new { mensagem = $"Transação com ID {id} deletada com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Transação não encontrada: {ex.Message}");
                return NotFound(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o resumo consolidado de receitas, despesas e saldos por pessoa
        /// Mostra o total de cada pessoa e o total geral
        /// </summary>
        /// <returns>Resumo por pessoa com totais consolidados</returns>
        [HttpGet("resumo/pessoa")]
        public async Task<ActionResult> ObterResumoPorPessoa()
        {
            var resumoPessoas = await _transacaoService.ObterResumoPorPessoaAsync();
            var totalGeral = await _transacaoService.ObterTotalGeralAsync();

            return Ok(new
            {
                pessoas = resumoPessoas,
                totalGeral = totalGeral
            });
        }

        /// <summary>
        /// Obtém o resumo consolidado de receitas, despesas e saldos por categoria
        /// Mostra o total de cada categoria e o total geral
        /// </summary>
        /// <returns>Resumo por categoria com totais consolidados</returns>
        [HttpGet("resumo/categoria")]
        public async Task<ActionResult> ObterResumoPorCategoria()
        {
            var resumoCategorias = await _transacaoService.ObterResumoPorCategoriaAsync();
            var totalGeral = await _transacaoService.ObterTotalGeralAsync();

            return Ok(new
            {
                categorias = resumoCategorias,
                totalGeral = totalGeral
            });
        }

        /// <summary>
        /// Obtém o total geral consolidado de todas as transações
        /// </summary>
        /// <returns>Totais consolidados</returns>
        [HttpGet("resumo/total")]
        public async Task<ActionResult<TotalGeralDTO>> ObterTotalGeral()
        {
            var totalGeral = await _transacaoService.ObterTotalGeralAsync();
            return Ok(totalGeral);
        }
    }
}
