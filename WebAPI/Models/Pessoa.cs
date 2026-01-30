namespace ControleGastosResidenciais.Models
{
    /// <summary>
    /// Modelo de dados para Pessoa (Residente)
    /// Representa uma pessoa cadastrada no sistema de controle de gastos
    /// </summary>
    public class Pessoa
    {
        /// <summary>
        /// Identificador único da pessoa, gerado automaticamente
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da pessoa (máximo 200 caracteres)
        /// Campo obrigatório
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Idade da pessoa em anos
        /// Utilizado para validar se pode ter receitas (apenas maiores de 18 anos)
        /// </summary>
        public int Idade { get; set; }

        /// <summary>
        /// Data de criação do registro
        /// Definida automaticamente ao criar a pessoa
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Relacionamento: Uma pessoa pode ter várias transações
        /// As transações são deletadas em cascata quando a pessoa é deletada
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
