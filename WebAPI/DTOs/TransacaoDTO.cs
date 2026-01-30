using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.DTOs
{
    /// <summary>
    /// DTO para criar uma Transação
    /// Valida se a transação é compatível com a categoria selecionada
    /// </summary>
    public class CriarTransacaoDTO
    {
        /// <summary>Descrição da transação (máximo 400 caracteres)</summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>Valor da transação (sempre positivo)</summary>
        public decimal Valor { get; set; }

        /// <summary>Tipo da transação (1=Despesa, 2=Receita)</summary>
        public EnumTipoTransacao Tipo { get; set; }

        /// <summary>ID da categoria para esta transação</summary>
        public int CategoriaId { get; set; }

        /// <summary>ID da pessoa que realizou esta transação</summary>
        public int PessoaId { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma Transação
    /// </summary>
    public class TransacaoDTO
    {
        /// <summary>Identificador único da transação</summary>
        public int Id { get; set; }

        /// <summary>Descrição da transação</summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>Valor da transação</summary>
        public decimal Valor { get; set; }

        /// <summary>Tipo da transação</summary>
        public EnumTipoTransacao Tipo { get; set; }

        /// <summary>ID da categoria</summary>
        public int CategoriaId { get; set; }

        /// <summary>Descrição da categoria</summary>
        public string CategoriaDescricao { get; set; } = string.Empty;

        /// <summary>ID da pessoa</summary>
        public int PessoaId { get; set; }

        /// <summary>Nome da pessoa</summary>
        public string PessoaNome { get; set; } = string.Empty;

        /// <summary>Data de criação da transação</summary>
        public DateTime DataCriacao { get; set; }
    }
}
