using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.DTOs
{
    /// <summary>
    /// DTO para criar ou atualizar uma Categoria
    /// </summary>
    public class CriarCategoriaDTO
    {
        /// <summary>Descrição da categoria (máximo 400 caracteres)</summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Finalidade da categoria (1=Despesa, 2=Receita, 3=Ambas)
        /// </summary>
        public EnumFinalidade Finalidade { get; set; }
    }

    /// <summary>
    /// DTO para retornar dados de uma Categoria
    /// </summary>
    public class CategoriaDTO
    {
        /// <summary>Identificador único da categoria</summary>
        public int Id { get; set; }

        /// <summary>Descrição da categoria</summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>Finalidade da categoria</summary>
        public EnumFinalidade Finalidade { get; set; }

        /// <summary>Data de criação do registro</summary>
        public DateTime DataCriacao { get; set; }
    }
}
