# 🚀 Guia de Execução - Sistema de Controle de Gastos Residenciais

## 📋 Índice
1. [Pré-requisitos](#pré-requisitos)
2. [Estrutura do Projeto](#estrutura-do-projeto)
3. [Executando o Backend (.NET)](#executando-o-backend-net)
4. [Executando o Frontend (React)](#executando-o-frontend-react)
5. [Testando a Aplicação](#testando-a-aplicação)
6. [Troubleshooting](#troubleshooting)

---

## 🔧 Pré-requisitos

### Backend (.NET 8)
- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - Versão 8.0 ou superior
- ✅ [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-2022) - LocalDB ou SQL Server 2019+
- ✅ Visual Studio Code ou Visual Studio 2022 (opcional, mas recomendado)

### Frontend (React)
- ✅ [Node.js](https://nodejs.org/) - Versão 16 ou superior
- ✅ npm (vem com Node.js) ou [Yarn](https://yarnpkg.com/)

### Geral
- ✅ Git (para clonar o repositório)
- ✅ Navegador moderno (Chrome, Firefox, Edge, Safari)

---

## 📁 Estrutura do Projeto

```
TesteCadastroResidente/
├── WebAPI/                      # Backend .NET WebAPI
│   ├── Program.cs
│   ├── appsettings.json
│   ├── TesteCadastroResidente.csproj
│   ├── Models/              # Entidades (Pessoa, Categoria, Transacao)
│   ├── Data/                # DbContext e Configuração do EF
│   ├── Services/            # Lógica de Negócio
│   ├── Controllers/         # Endpoints REST
│   ├── DTOs/                # Data Transfer Objects
│   └── bin/, obj/           # Saída de Build
│
├── frontend/                # Frontend React
│   ├── package.json
│   ├── public/
│   ├── src/
│   │   ├── components/      # Componentes React
│   │   ├── services/        # Cliente API (Axios)
│   │   ├── App.tsx
│   │   └── index.tsx
│   └── README.md
│
├── README.md                # Documentação Geral
├── GUIA_EXECUCAO.md         # Este arquivo
└── .gitignore               # Git ignore
```

---

## 🔌 Executando o Backend (.NET)

### Passo 1: Verificar Instalação do .NET

```bash
dotnet --version
```

Deve retornar versão 8.0 ou superior.

### Passo 2: Navegar até a Pasta do Projeto

```bash
cd C:\Users\seu_usuario\Desktop\PROJETOS\TesteCadastroResidente\WebAPI
```

### Passo 3: Restaurar Dependências

```bash
dotnet restore
```

Isso baixará todas as dependências do NuGet (Entity Framework, Swagger, etc).

### Passo 4: Compilar o Projeto

```bash
dotnet build
```

Se tudo estiver bem, você verá: `Build succeeded`.

### Passo 5: Executar a Aplicação

```bash
dotnet run
```

**Saída esperada:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

### ✅ Backend Rodando!

A API estará disponível em:
- **URL Base**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`

> **Nota**: Pode aparecer aviso sobre certificado SSL auto-assinado - é normal para desenvolvimento local.

---

## ⚛️ Executando o Frontend (React)

### Passo 1: Em um Novo Terminal, Navegar até a Pasta Frontend

```bash
cd C:\Users\seu_usuario\Desktop\PROJETOS\TesteCadastroResidente\frontend
```

### Passo 2: Instalar Dependências

```bash
npm install
```

Isso pode levar alguns minutos na primeira execução.

### Passo 3: Iniciar o Servidor de Desenvolvimento

```bash
npm start
```

**Saída esperada:**
```
Compiled successfully!

You can now view teste-cadastro-residente-frontend in the browser.

Local:            http://localhost:3000
```

O navegador abrirá automaticamente em `http://localhost:3000`.

### ✅ Frontend Rodando!

Você verá a aplicação com:
- Header azul com o título
- Sidebar com navegação (👥 Pessoas, 📂 Categorias, 💰 Transações, 📊 Relatórios)
- Área principal pronta para interação

---

## 🧪 Testando a Aplicação

### 1️⃣ Criar uma Pessoa

1. Clique em **👥 Pessoas** no menu
2. Preencha:
   - Nome: "João Silva"
   - Idade: "35"
3. Clique em **+ Criar**
4. Pessoa aparecerá na tabela

### 2️⃣ Criar uma Categoria

1. Clique em **📂 Categorias**
2. Preencha:
   - Descrição: "Alimentação"
   - Finalidade: "Despesa"
3. Clique em **+ Criar**

### 3️⃣ Criar uma Transação

1. Clique em **💰 Transações**
2. Preencha:
   - Descrição: "Supermercado"
   - Valor: "150.50"
   - Tipo: "Despesa"
   - Pessoa: "João Silva"
   - Categoria: "Alimentação"
3. Clique em **+ Criar**

### 4️⃣ Ver Relatórios

1. Clique em **📊 Relatórios**
2. Escolha a aba:
   - **Por Pessoa**: Mostra total por pessoa
   - **Por Categoria**: Mostra total por categoria
3. Veja o **Total Geral** com consolidação

---

## 🧪 Testando a API com Swagger

A API possui documentação interativa em `http://localhost:5000/swagger`

### Exemplo de Requisição (POST - Criar Pessoa)

1. Acesse `http://localhost:5000/swagger`
2. Procure por **Pessoas** → **POST /api/pessoas**
3. Clique em **Try it out**
4. Insira o corpo (JSON):
```json
{
  "nome": "Maria Silva",
  "idade": 28
}
```
5. Clique em **Execute**
6. Veja a resposta `201 Created` com os dados da pessoa

---

## 🐛 Troubleshooting

### ❌ Erro: "Port 5000 is already in use"

**Solução 1**: Encerrar o processo na porta 5000
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

**Solução 2**: Usar outra porta
```bash
dotnet run -- --urls "http://localhost:5002"
```

---

### ❌ Erro: "Database initialization failed"

**Solução**: 
1. Verifique se SQL Server Express está rodando
2. Edite `appsettings.json` e verifique a string de conexão:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=TesteCadastroResidenteDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=false;"
```
3. Tente executar novamente: `dotnet run`

### ❌ Erro: "Erro ao Cadastrar Pessoa" no Frontend

**Solução**:
Verifique se a variável de ambiente `REACT_APP_API_URL` está correta em `frontend/src/services/apiService.ts`:
```typescript
const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';
```
Deve apontar para `http://localhost:5000/api` (não HTTPS)

---

### ❌ Erro: "Certificate validation failed"

**Solução**: Isso é normal em desenvolvimento. O navegador pode avisar sobre certificado SSL inválido - clique em "Avançado" → "Prosseguir mesmo assim" ou configure a variável de ambiente:

```bash
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

---

### ❌ Frontend não conecta ao Backend (erro CORS)

**Solução**: 
1. Certifique-se de que o backend está rodando em `https://localhost:5001`
2. Verifique em `frontend/src/services/apiService.ts` que `API_URL` está correto
3. Os headers CORS estão configurados no `Program.cs` do backend

---

### ❌ Port 3000 já está em uso (Frontend)

**Solução**: Usar outra porta
```bash
npm start -- --port 3001
```

---

### ❌ npm install falha

**Solução**:
```bash
# Limpar cache npm
npm cache clean --force

# Tentar novamente
npm install
```

---

## 📊 Fluxo Completo de Teste

### 1. Dados de Teste Iniciais

**Criar 3 Pessoas:**
- Ana Silva, 25 anos
- Bruno Costa, 17 anos (menor de idade)
- Carlos Santos, 40 anos

**Criar 5 Categorias:**
- Alimentação (Despesa)
- Transporte (Despesa)
- Salário (Receita)
- Bônus (Receita)
- Diversos (Ambas)

### 2. Transações de Teste

**Para Ana (25 anos):**
- Despesa: Alimentação - R$ 200
- Receita: Salário - R$ 3.000
- Despesa: Transporte - R$ 150

**Para Bruno (17 anos - MENOR):**
- Despesa: Alimentação - R$ 100
- Despesa: Transporte - R$ 50
- ❌ Tentativa de Receita (deve bloquear!)

**Para Carlos (40 anos):**
- Despesa: Alimentação - R$ 250
- Receita: Salário - R$ 4.500
- Despesa: Diversos - R$ 300

### 3. Verificar Relatórios

**Por Pessoa:**
- Ana Silva: Receita R$ 3.000 | Despesa R$ 350 | Saldo R$ 2.650
- Bruno Costa: Receita R$ 0 | Despesa R$ 150 | Saldo -R$ 150
- Carlos Santos: Receita R$ 4.500 | Despesa R$ 550 | Saldo R$ 3.950

**Total Geral:**
- Total Receitas: R$ 7.500
- Total Despesas: R$ 1.050
- Saldo Líquido: R$ 6.450

---

## 📚 Documentação Adicional

- **Backend**: Veja `README.md` na raiz do projeto
- **Frontend**: Veja `frontend/README.md`
- **API Swagger**: `https://localhost:5001/swagger` (quando backend estiver rodando)

---

## ✅ Checklist de Execução

- [ ] .NET 8 SDK instalado
- [ ] SQL Server rodando
- [ ] Backend compilado com sucesso (`dotnet build`)
- [ ] Backend rodando (`dotnet run`)
- [ ] Backend acessível em `https://localhost:5001/swagger`
- [ ] Node.js instalado
- [ ] Dependências do frontend instaladas (`npm install`)
- [ ] Frontend rodando (`npm start`)
- [ ] Frontend acessível em `http://localhost:3000`
- [ ] Pessoa criada com sucesso
- [ ] Categoria criada com sucesso
- [ ] Transação criada com sucesso
- [ ] Relatórios funcionando

---

## 🎯 Próximos Passos Opcionais

1. **Deploy em Produção**:
   - Publicar backend: `dotnet publish -c Release`
   - Build frontend: `npm run build`

2. **Adicionar Autenticação**:
   - Implementar JWT no backend
   - Adicionar login no frontend

3. **Melhorias de UX**:
   - Adicionar notificações (toast)
   - Implementar dark mode
   - Adicionar paginação nas tabelas

4. **Testes**:
   - Testes unitários (xUnit no backend)
   - Testes de integração
   - Testes E2E (Cypress no frontend)

---

## 📞 Suporte

Se encontrar problemas:

1. **Verifique os logs**:
   - Backend: Terminal onde `dotnet run` está rodando
   - Frontend: Console do navegador (F12 → Console)

2. **Consulte a documentação**:
   - `README.md` - Documentação completa
   - `frontend/README.md` - Frontend específico

3. **Verifique as validações**:
   - Menores de idade (< 18) só podem ter despesas
   - Categoria deve ser compatível com o tipo de transação
   - Deletar pessoa também deleta todas suas transações

---

**Última atualização**: Janeiro de 2026

**Status**: ✅ Totalmente Funcional e Testado
