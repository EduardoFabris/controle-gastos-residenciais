using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Models;

namespace ControleGastosResidenciais.Data
{
    /// <summary>
    /// Contexto do banco de dados para a aplicação de controle de gastos residenciais
    /// Gerencia todas as operações de persistência de dados via Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Construtor que aceita as opções de configuração do DbContext
        /// </summary>
        /// <param name="options">Opções de configuração do contexto</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet para operações com a entidade Pessoa
        /// Permite consultas, inserções, atualizações e deleções de pessoas
        /// </summary>
        public DbSet<Pessoa> Pessoas { get; set; }

        /// <summary>
        /// DbSet para operações com a entidade Categoria
        /// Permite consultas, inserções, atualizações e deleções de categorias
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// DbSet para operações com a entidade Transacao
        /// Permite consultas, inserções, atualizações e deleções de transações
        /// </summary>
        public DbSet<Transacao> Transacoes { get; set; }

        /// <summary>
        /// Configura as entidades e seus relacionamentos
        /// Define comportamentos de exclusão em cascata e outras configurações do banco de dados
        /// </summary>
        /// <param name="modelBuilder">Builder para configuração do modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Pessoa
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Idade)
                    .IsRequired();

                entity.Property(e => e.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento: Uma pessoa tem muitas transações
                // OnDelete: Cascade - Quando uma pessoa é deletada, todas as suas transações são deletadas
                entity.HasMany(e => e.Transacoes)
                    .WithOne(t => t.Pessoa)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Finalidade)
                    .IsRequired();

                entity.Property(e => e.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relacionamento: Uma categoria tem muitas transações
                entity.HasMany(e => e.Transacoes)
                    .WithOne(t => t.Categoria)
                    .HasForeignKey(t => t.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade Transacao
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.Valor)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(e => e.Tipo)
                    .IsRequired();

                entity.Property(e => e.CategoriaId)
                    .IsRequired();

                entity.Property(e => e.PessoaId)
                    .IsRequired();

                entity.Property(e => e.DataCriacao)
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Índices para melhorar performance em buscas
                entity.HasIndex(e => e.PessoaId);
                entity.HasIndex(e => e.CategoriaId);
            });
        }
    }
}
