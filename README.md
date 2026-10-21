# 📊 Sistema de Controle de Gastos Residenciais

Um sistema profissional de controle de gastos residenciais desenvolvido com **.NET 8** e **React TypeScript**, separado em WebAPI e Frontend com banco de dados SQL Server. Desenvolvido por **Eduardo Fabris**.

## 🎯 Funcionalidades

### ✅ Gerenciamento de Pessoas
- **Criar**: Cadastrar nova pessoa com nome (máx 200 caracteres) e idade
- **Listar**: Visualizar todas as pessoas cadastradas ordenadas por nome
- **Atualizar**: Modificar dados de uma pessoa existente
- **Deletar**: Remover pessoa e todas suas transações em cascata

### ✅ Gerenciamento de Categorias
- **Criar**: Cadastrar categoria com descrição (máx 400 caracteres) e finalidade
  - **Finalidades**: Despesa, Receita ou Ambas
- **Listar**: Visualizar todas as categorias ordenadas por descrição
- **Atualizar**: Modificar dados de uma categoria
- **Deletar**: Remover categoria (não permite se houver transações)

### ✅ Gerenciamento de Transações
- **Criar**: Registrar nova transação com validações completas
  - Pessoa e categoria devem existir
  - Valor deve ser positivo
  - **Menores de idade (< 18 anos) só podem ter despesas**
  - Categoria deve ser compatível com o tipo da transação
- **Listar**: Visualizar todas as transações ou filtrar por pessoa
- **Atualizar**: Modificar dados de uma transação com validações
- **Deletar**: Remover uma transação

### ✅ Consultas e Relatórios
- **Resumo por Pessoa**: Total de receitas, despesas e saldo por pessoa
  - Inclui total geral consolidado
- **Resumo por Categoria**: Total de receitas, despesas e saldo por categoria
  - Inclui total geral consolidado
- **Total Geral**: Somatório de todas as transações, pessoas e categorias

---

## 🛠️ Stack Tecnológico

### Backend
- **Framework**: ASP.NET Core 8.0 WebAPI
- **ORM**: Entity Framework Core 8.0 com SQL Server
- **Linguagem**: C# 12
- **Documentação API**: Swagger/Swashbuckle 6.5.0
- **Padrões**: Dependency Injection, Service Layer Pattern

### Frontend
- **Framework**: React 18.2.0 com TypeScript 4.9.5
- **HTTP Client**: Axios 1.6.0
- **Build Tool**: react-scripts 5.0.1
- **Otimizações**: useCallback para React Hooks Performance

### Banco de Dados
- **SGBD**: SQL Server Express (`.\SQLEXPRESS`)
- **Padrões de Relacionamento**: Cascade Delete (Pessoa→Transação), Restrict Delete (Categoria→Transação)

---

## 🏗️ Arquitetura do Projeto

### Backend (.NET 8 WebAPI)

```
WebAPI/
├── Models/
│   ├── Pessoa.cs          # Modelo de dados de pessoas
│   ├── Categoria.cs       # Modelo de dados de categorias
│   └── Transacao.cs       # Modelo de dados de transações
├── Data/
│   └── ApplicationDbContext.cs  # Configuração do Entity Framework
├── DTOs/
│   ├── PessoaDTO.cs
│   ├── CategoriaDTO.cs
│   ├── TransacaoDTO.cs
│   └── ResumoDTO.cs       # DTOs para resumos e totais
├── Services/
│   ├── IPessoaService & PessoaService
│   ├── ICategoriaService & CategoriaService
│   └── ITransacaoService & TransacaoService
├── Controllers/
│   ├── PessoasController
│   ├── CategoriasController
│   └── TransacoesController
├── Program.cs             # Configuração da aplicação
└── appsettings.json       # Configurações (conexão DB, etc)
```

### Camadas Implementadas

1. **Models**: Entidades de domínio com relacionamentos e validações
2. **DbContext**: Configuração do Entity Framework Core com SQL Server
3. **DTOs**: Objetos de transferência de dados (separação entre API e modelos)
4. **Services**: Lógica de negócio e validações
5. **Controllers**: Endpoints REST API com tratamento de erros
6. **Dependency Injection**: Integração nativa do .NET

---

## 🔌 API REST Endpoints

### 👥 Pessoas
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/pessoas` | Criar nova pessoa |
| GET | `/api/pessoas` | Listar todas as pessoas |
| GET | `/api/pessoas/{id}` | Obter pessoa por ID |
| PUT | `/api/pessoas/{id}` | Atualizar pessoa |
| DELETE | `/api/pessoas/{id}` | Deletar pessoa |

### 📂 Categorias
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/categorias` | Criar nova categoria |
| GET | `/api/categorias` | Listar todas as categorias |
| GET | `/api/categorias/{id}` | Obter categoria por ID |
| PUT | `/api/categorias/{id}` | Atualizar categoria |
| DELETE | `/api/categorias/{id}` | Deletar categoria |

### 💰 Transações
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/transacoes` | Criar nova transação |
| GET | `/api/transacoes` | Listar todas as transações |
| GET | `/api/transacoes/{id}` | Obter transação por ID |
| GET | `/api/transacoes/pessoa/{pessoaId}` | Listar transações da pessoa |
| PUT | `/api/transacoes/{id}` | Atualizar transação |
| DELETE | `/api/transacoes/{id}` | Deletar transação |

### 📊 Resumos e Totais
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/transacoes/resumo/pessoa` | Resumo por pessoa com totais |
| GET | `/api/transacoes/resumo/categoria` | Resumo por categoria com totais |
| GET | `/api/transacoes/resumo/total` | Total geral consolidado |

---

## 💾 Modelo de Dados

### Pessoa
```csharp
{
    id: int,                    // Auto-gerado
    nome: string (max 200),     // Obrigatório
    idade: int,                 // Obrigatório
    dataCriacao: DateTime       // Auto-preenchido
}
```

### Categoria
```csharp
{
    id: int,                    // Auto-gerado
    descricao: string (max 400), // Obrigatório
    finalidade: enum,           // 1=Despesa, 2=Receita, 3=Ambas
    dataCriacao: DateTime       // Auto-preenchido
}
```

### Transação
```csharp
{
    id: int,                    // Auto-gerado
    descricao: string (max 400), // Obrigatório
    valor: decimal (positivo),  // Obrigatório
    tipo: enum,                 // 1=Despesa, 2=Receita
    categoriaId: int,           // FK - deve ser compatível com tipo
    pessoaId: int,              // FK
    dataCriacao: DateTime       // Auto-preenchido
}
```

---

## 🔒 Validações e Regras de Negócio

### Pessoas
- ✅ Nome não pode estar vazio ou exceder 200 caracteres
- ✅ Idade deve ser um valor positivo
- ✅ Ao deletar uma pessoa, todas suas transações são deletadas em cascata

### Categorias
- ✅ Descrição não pode estar vazia ou exceder 400 caracteres
- ✅ Finalidade deve ser válida (Despesa, Receita ou Ambas)
- ✅ Não é possível deletar uma categoria que possui transações

### Transações
- ✅ Descrição não pode estar vazia ou exceder 400 caracteres
- ✅ Valor deve ser sempre maior que zero
- ✅ Pessoa deve estar cadastrada
- ✅ Categoria deve estar cadastrada
- ✅ **Menores de idade (< 18 anos) não podem ter transações de receita**
- ✅ Categoria deve ser compatível com o tipo (despesa/receita)

---

## 🗄️ Banco de Dados

### SQL Server
- **Nome**: `TesteCadastroResidenteDb`
- **Autenticação**: Windows (Trusted Connection)
- **Tabelas**: Pessoas, Categorias, Transacoes
- **Relacionamentos**: 
  - Pessoa ↔ Transacao (1:N) - Cascade Delete
  - Categoria ↔ Transacao (1:N) - Restrict Delete

### String de Conexão
```
Server=.\SQLEXPRESS;Database=TesteCadastroResidenteDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=false;
```

---

## 🚀 Executando o Projeto

### Backend (.NET)

#### Pré-requisitos
- .NET 8 SDK instalado
- SQL Server (LocalDB ou completo)
- Visual Studio Code ou Visual Studio

#### Passos
1. Navegue até a pasta do projeto
2. Restaure as dependências:
   ```bash
   dotnet restore
   ```
3. Compile o projeto:
   ```bash
   dotnet build
   ```
4. Execute a aplicação:
   ```bash
   dotnet run
   ```
   - A API estará disponível em: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

---

## 📝 Exemplos de Uso da API

### 1. Criar uma Pessoa
```bash
POST /api/pessoas
Content-Type: application/json

{
    "nome": "João Silva",
    "idade": 35
}

# Response 201 Created
{
    "id": 1,
    "nome": "João Silva",
    "idade": 35,
    "dataCriacao": "2026-01-30T10:30:00Z"
}
```

### 2. Criar uma Categoria
```bash
POST /api/categorias
Content-Type: application/json

{
    "descricao": "Alimentação",
    "finalidade": 1  // 1=Despesa
}

# Response 201 Created
{
    "id": 1,
    "descricao": "Alimentação",
    "finalidade": 1,
    "dataCriacao": "2026-01-30T10:30:00Z"
}
```

### 3. Criar uma Transação
```bash
POST /api/transacoes
Content-Type: application/json

{
    "descricao": "Supermercado",
    "valor": 150.50,
    "tipo": 1,          // 1=Despesa, 2=Receita
    "categoriaId": 1,
    "pessoaId": 1
}

# Response 201 Created
{
    "id": 1,
    "descricao": "Supermercado",
    "valor": 150.50,
    "tipo": 1,
    "categoriaId": 1,
    "categoriaDescricao": "Alimentação",
    "pessoaId": 1,
    "pessoaNome": "João Silva",
    "dataCriacao": "2026-01-30T10:30:00Z"
}
```

### 4. Resumo por Pessoa
```bash
GET /api/transacoes/resumo/pessoa

# Response 200 OK
{
    "pessoas": [
        {
            "pessoaId": 1,
            "pessoaNome": "João Silva",
            "idade": 35,
            "totalReceitas": 5000.00,
            "totalDespesas": 1500.50,
            "saldo": 3499.50
        }
    ],
    "totalGeral": {
        "totalReceitas": 5000.00,
        "totalDespesas": 1500.50,
        "saldoLiquido": 3499.50,
        "qtdTransacoes": 10,
        "qtdPessoas": 1,
        "qtdCategorias": 5
    }
}
```

---

## 📚 Tecnologias Utilizadas

### Backend
- **Framework**: .NET 8
- **Linguagem**: C# 12
- **Banco de Dados**: SQL Server
- **ORM**: Entity Framework Core 8
- **API**: ASP.NET Core Web API
- **Documentação**: Swagger/OpenAPI (Swashbuckle)
- **Logging**: ILogger (nativo do .NET)

### Frontend (Em desenvolvimento)
- **Framework**: React
- **Linguagem**: TypeScript
- **HTTP Client**: Axios
- **State Management**: React Query
- **UI Components**: A definir
- **Styling**: A definir

---

## 🔐 Segurança e Boas Práticas

✅ **Validação de Entrada**: Todas as entradas são validadas antes de persistência  
✅ **Tratamento de Erros**: Erros são capturados e retornados de forma estruturada  
✅ **Logging**: Operações importantes são registradas  
✅ **CORS**: Configurado para aceitar requisições do frontend  
✅ **Injeção de Dependência**: Padrão DI implementado em toda a aplicação  
✅ **Separação de Camadas**: Models, Services, Controllers e DTOs bem definidos  
✅ **Documentação XML**: Comentários completos em todas as classes públicas  

---

## 📖 Documentação do Código

Toda a codebase possui documentação XML detalhada:

- ✅ Classes e suas responsabilidades
- ✅ Métodos e seus parâmetros
- ✅ Valores de retorno
- ✅ Exceções que podem ser lançadas
- ✅ Lógica de negócio explicada nos comentários

### Visualizar Documentação
A documentação é gerada automaticamente no Intellisense do Visual Studio e VS Code.

---

## 📦 Estrutura de Pastas do Projeto

```
TesteCadastroResidente/
├── WebAPI/                          # Backend .NET WebAPI
│   ├── Controllers/                 # Endpoints REST API
│   ├── Data/                        # Entity Framework DbContext
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Models/                      # Entidades de domínio
│   ├── Services/                    # Lógica de negócio
│   ├── Program.cs                   # Entry point
│   ├── appsettings.json             # Configurações
│   └── TesteCadastroResidente.csproj # Projeto .NET
│
├── frontend/                        # Frontend React TypeScript
│   ├── public/                      # Assets públicos
│   ├── src/
│   │   ├── components/              # Componentes React
│   │   ├── services/                # Cliente API (Axios)
│   │   ├── App.tsx
│   │   ├── index.tsx
│   │   └── *.css                    # Estilos
│   ├── package.json
│   ├── tsconfig.json
│   └── tsconfig.node.json
│
├── README.md                        # Documentação Geral
├── GUIA_EXECUCAO.md                 # Guia de como executar
└── .gitignore                       # Git ignore
```

---

## 🐛 Tratamento de Erros

A API retorna respostas estruturadas para erros:

```json
{
    "erro": "Descrição do erro ocorrido"
}
```

### Códigos HTTP Utilizados
- `200 OK`: Requisição bem-sucedida
- `201 Created`: Recurso criado com sucesso
- `400 Bad Request`: Validação falhou ou dados inválidos
- `404 Not Found`: Recurso não encontrado
- `500 Internal Server Error`: Erro interno do servidor

---

## 📋 Próximas Etapas

- [ ] Implementar autenticação e autorização
- [ ] Adicionar testes unitários (xUnit)
- [ ] Implementar pagination nas listagens
- [ ] Adicionar filtros avançados
- [ ] Implementar relatórios em PDF
- [ ] Deploy em produção
- [ ] Configurar CI/CD (Azure DevOps)

---

## 👨‍💻 Autor

Sistema desenvolvido por Eduardo Fabris como solução completa de controle de gastos residenciais.

**Data**: Janeiro de 2026

---

## 📄 Licença

Este projeto é fornecido como está, sem garantias.

---

## 📞 Suporte

Para dúvidas ou sugestões, consulte a documentação do código ou entre em contato com o desenvolvedor.
