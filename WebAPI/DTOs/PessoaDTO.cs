namespace ControleGastosResidenciais.DTOs
{
    /// <summary>
    /// DTO (Data Transfer Object) para criar ou atualizar uma Pessoa
    /// Transfere dados entre o cliente e a API
    /// </summary>
    public class CriarPessoaDTO
    {
        /// <summary>Nome da pessoa (máximo 200 caracteres)</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Idade da pessoa em anos</summary>
        public int Idade { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma Pessoa
    /// Inclui todas as informações necessárias para o cliente
    /// </summary>
    public class PessoaDTO
    {
        /// <summary>Identificador único da pessoa</summary>
        public int Id { get; set; }

        /// <summary>Nome da pessoa</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Idade da pessoa</summary>
        public int Idade { get; set; }

        /// <summary>Data de criação do registro</summary>
        public DateTime DataCriacao { get; set; }
    }
}
