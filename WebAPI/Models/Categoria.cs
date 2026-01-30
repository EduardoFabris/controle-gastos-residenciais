namespace ControleGastosResidenciais.Models
{
    /// <summary>
    /// Modelo de dados para Categoria
    /// Representa um tipo de categoria para classificar transações
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único da categoria, gerado automaticamente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da categoria (máximo 400 caracteres)
        /// Exemplos: Alimentação, Transporte, Diversão, Salário, Bônus
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria: Despesa, Receita ou Ambas
        /// Usado para validar se uma transação pode usar esta categoria
        /// </summary>
        public EnumFinalidade Finalidade { get; set; }

        /// <summary>
        /// Data de criação do registro
        /// Definida automaticamente ao criar a categoria
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Relacionamento: Uma categoria pode ter várias transações
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }

    /// <summary>
    /// Enumeração para definir a finalidade de uma categoria
    /// </summary>
    public enum EnumFinalidade
    {
        /// <summary>Categoria para despesas (gastos)</summary>
        Despesa = 1,

        /// <summary>Categoria para receitas (entradas)</summary>
        Receita = 2,

        /// <summary>Categoria para ambas despesas e receitas</summary>
        Ambas = 3
    }
}
