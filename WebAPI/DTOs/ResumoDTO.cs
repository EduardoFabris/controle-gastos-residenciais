using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.DTOs
{
    /// <summary>
    /// DTO para retornar o resumo de transações por pessoa
    /// Inclui totais de receitas, despesas e saldo
    /// </summary>
    public class ResumoPorPessoaDTO
    {
        /// <summary>Identificador da pessoa</summary>
        public int PessoaId { get; set; }

        /// <summary>Nome da pessoa</summary>
        public string PessoaNome { get; set; } = string.Empty;

        /// <summary>Idade da pessoa</summary>
        public int Idade { get; set; }

        /// <summary>Total de receitas da pessoa</summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>Total de despesas da pessoa</summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>Saldo da pessoa (receitas - despesas)</summary>
        public decimal Saldo { get; set; }
    }

    /// <summary>
    /// DTO para retornar o resumo de transações por categoria
    /// Inclui totais de receitas, despesas e saldo
    /// </summary>
    public class ResumoPorCategoriaDTO
    {
        /// <summary>Identificador da categoria</summary>
        public int CategoriaId { get; set; }

        /// <summary>Descrição da categoria</summary>
        public string CategoriaDescricao { get; set; } = string.Empty;

        /// <summary>Finalidade da categoria (Despesa, Receita, Ambas)</summary>
        public EnumFinalidade Finalidade { get; set; }

        /// <summary>Total de receitas da categoria</summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>Total de despesas da categoria</summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>Saldo da categoria (receitas - despesas)</summary>
        public decimal Saldo { get; set; }
    }

    /// <summary>
    /// DTO para retornar o total geral consolidado
    /// Inclui somatório de todas as transações, pessoas e categorias
    /// </summary>
    public class TotalGeralDTO
    {
        /// <summary>Total de todas as receitas</summary>
        public decimal TotalReceitas { get; set; }

        /// <summary>Total de todas as despesas</summary>
        public decimal TotalDespesas { get; set; }

        /// <summary>Saldo líquido geral (receitas - despesas)</summary>
        public decimal SaldoLiquido { get; set; }

        /// <summary>Quantidade total de transações registradas</summary>
        public int QtdTransacoes { get; set; }

        /// <summary>Quantidade total de pessoas cadastradas</summary>
        public int QtdPessoas { get; set; }

        /// <summary>Quantidade total de categorias cadastradas</summary>
        public int QtdCategorias { get; set; }
    }
}
