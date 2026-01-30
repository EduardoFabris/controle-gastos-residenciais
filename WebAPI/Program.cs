using Microsoft.EntityFrameworkCore;
using ControleGastosResidenciais.Data;
using ControleGastosResidenciais.Services;

// Criação e configuração do builder da aplicação
var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURAÇÃO DE SERVIÇOS
// ============================================

// Adiciona os services de logging, Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS para permitir requisições do frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Permite requisições do frontend React rodando em localhost:3000
        // Também permite de qualquer origem durante desenvolvimento
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ============================================

// Obtém a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext com Entity Framework Core e SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
        sqlOptions => sqlOptions.MigrationsAssembly("ControleGastosResidenciais"))
);

// ============================================
// INJEÇÃO DE DEPENDÊNCIAS (Services)
// ============================================

// Registra os serviços de negócio para injeção de dependência
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// ============================================
// CONSTRUÇÃO DA APLICAÇÃO
// ============================================

var app = builder.Build();

// ============================================
// PIPELINE DE MIDDLEWARE
// ============================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Habilita Swagger/OpenAPI apenas em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona HTTP para HTTPS
app.UseHttpsRedirection();

// Habilita CORS com a política configurada
app.UseCors("AllowFrontend");

// Ativa o middleware de autenticação e autorização
app.UseAuthorization();

// Mapeia os controllers
app.MapControllers();

// ============================================
// CRIAÇÃO AUTOMÁTICA DO BANCO DE DADOS
// ============================================

// Executa as migrations pendentes e cria o banco se não existir
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Cria o banco de dados caso não exista
    dbContext.Database.EnsureCreated();
    
    // Alterna: usar migrations automáticas
    // dbContext.Database.Migrate();
}

// ============================================
// EXECUÇÃO DA APLICAÇÃO
// ============================================

app.Run();
