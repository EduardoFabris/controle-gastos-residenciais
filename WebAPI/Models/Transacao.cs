namespace ControleGastosResidenciais.Models
{
    /// <summary>
    /// Modelo de dados para Transação
    /// Representa uma transação financeira (receita ou despesa) de uma pessoa
    /// </summary>
    public class Transacao
    {
        /// <summary>
        /// Identificador único da transação, gerado automaticamente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Descrição da transação (máximo 400 caracteres)
        /// Exemplos: "Compra de alimentos", "Salário do mês", "Pagamento de aluguel"
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação (sempre positivo)
        /// O tipo (despesa ou receita) é determinado pelo campo Tipo
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Tipo da transação: Despesa ou Receita
        /// Determina se é uma saída ou entrada de dinheiro
        /// </summary>
        public EnumTipoTransacao Tipo { get; set; }

        /// <summary>
        /// Identificador da categoria
        /// Referência para qual categoria esta transação pertence
        /// A categoria deve ter uma finalidade compatível com o tipo
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Relacionamento: Referência para a categoria
        /// </summary>
        public Categoria? Categoria { get; set; }

        /// <summary>
        /// Identificador da pessoa
        /// Referência para qual pessoa realizou esta transação
        /// </summary>
        public int PessoaId { get; set; }

        /// <summary>
        /// Relacionamento: Referência para a pessoa
        /// </summary>
        public Pessoa? Pessoa { get; set; }

        /// <summary>
        /// Data de criação do registro da transação
        /// Definida automaticamente
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Enumeração para definir o tipo de uma transação
    /// </summary>
    public enum EnumTipoTransacao
    {
        /// <summary>Transação de saída de dinheiro</summary>
        Despesa = 1,

        /// <summary>Transação de entrada de dinheiro</summary>
        Receita = 2
    }
}
