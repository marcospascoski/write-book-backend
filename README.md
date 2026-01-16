# 📚 WriteBook API

API backend para o sistema WriteBook - Plataforma de gerenciamento de livros e autores.

## 🚀 Início Rápido

### Instalação Automatizada (Recomendado)

```bash
cd D:\Dev\write-book-backend
chmod +x instalador.sh
./instalador.sh
```

Escolha a **Opção 1** para instalação completa com wizard interativo!

### Instalação Manual

Consulte o arquivo [`QUICK_START.md`](QUICK_START.md) para instruções detalhadas.

## 📋 Pré-requisitos

- Docker Desktop
- Docker Compose
- Git
- 8GB RAM
- 10GB espaço em disco

## 🔧 Utiliza projeto Onix.Framework na diretorio Raiz(write-book-backend)
link: https://github.com/marcospascoski/Onix.Framework


## 🏗️ Arquitetura

- **.NET 8** - Framework principal
- **PostgreSQL 16** - Banco de dados
- **Entity Framework Core** - ORM
- **JWT** - Autenticação
- **Docker** - Containerização
- **Swagger** - Documentação da API

## 📁 Estrutura do Projeto

```
write-book-backend/
├── instalador.sh                    # Script de instalação interativo
├── docker-compose.writebook.yml     # Configuração Docker Compose
├── .env.example                     # Template de variáveis
├── QUICK_START.md                   # Guia de início rápido
├── PORTAINER_DEPLOYMENT.md          # Deploy em produção
├── src/
│   └── backend/
│       ├── Acesso/                  # Módulo de autenticação
│       ├── Books/                   # Módulo de livros
│       ├── Core/                    # Funcionalidades core
│       ├── Sistema/                 # Configurações do sistema
│       └── WebApi/                  # API REST
│           └── Onix.Writebook.WebApi/
│               ├── Dockerfile
│               ├── DbMigrationRunner.cs
│               └── Program.cs
└── Onix.Framework/                  # Framework base
```

## 🔧 Funcionalidades

### Módulo Acesso
- ✅ Autenticação JWT
- ✅ Gerenciamento de usuários
- ✅ Perfis e permissões
- ✅ Controle de acesso

### Módulo Books
- ✅ Exportar pdf

### Módulo Core
- ✅ Notificações
- ✅ Email service
- ✅ Recursos compartilhados

### Módulo Sistema
- ✅ Configurações gerais
- ✅ Parâmetros do sistema

## 🌐 Endpoints Principais

Após iniciar a aplicação, acesse:

- **API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health

## 🔐 Segurança

### Desenvolvimento
```env
DB_PASSWORD=postgres
JWT_SECRET=dev-secret-key
```

### Produção
- ⚠️ **Nunca use credenciais padrão**
- ✅ Gere JWT Secret forte (64+ caracteres)
- ✅ Use senhas complexas no banco
- ✅ Configure HTTPS/TLS
- ✅ Revise CORS policies

## 🐳 Docker

### Build da Imagem
```bash
docker build -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t writebook-api:latest .
```

### Executar Localmente
```bash
docker compose -f docker-compose.writebook.yml up -d
```

### Ver Logs
```bash
docker compose -f docker-compose.writebook.yml logs -f
```

### Parar Serviços
```bash
docker compose -f docker-compose.writebook.yml down
```

## 💾 Banco de Dados

### Migrations

As migrations são aplicadas automaticamente na primeira inicialização quando `APPLY_MIGRATIONS=true`.

**Importante:** Após primeira instalação, a variável é alterada automaticamente para `false`.

### Backup
```bash
docker exec $(docker ps -q -f name=postgres) pg_dump -U postgres db-write-book > backup.sql
```

### Restore
```bash
cat backup.sql | docker exec -i $(docker ps -q -f name=postgres) psql -U postgres db-write-book
```

## 🔄 Atualização

Para atualizar para uma nova versão:

```bash
./instalador.sh
# Escolha a Opção 2 (Atualizar API)
```

O instalador irá:
1. Fazer backup automático do banco
2. Rebuild da imagem
3. Perguntar sobre novas migrations
4. Redeploy com zero-downtime

## 📊 Monitoramento

### Ver Status
```bash
docker compose -f docker-compose.writebook.yml ps
```

### Estatísticas de Uso
```bash
docker stats
```

### Verificar Banco
```bash
docker exec -it $(docker ps -q -f name=postgres) psql -U postgres db-write-book
\dt  # Lista tabelas
\q   # Sair
```

## 🧪 Desenvolvimento

### Estrutura de DbContexts

O projeto utiliza 3 DbContexts separados:

1. **SistemaDbContext** - Configurações do sistema
2. **AcessosDbContext** - Autenticação e autorização
3. **BooksDbContext** - Gerenciamento de livros

### Adicionar Nova Migration

```bash
# Sistema
dotnet ef migrations add NomeDaMigration -c SistemaDbContext -o Migrations

# Acesso
dotnet ef migrations add NomeDaMigration -c AcessosDbContext -o Migrations

# Books
dotnet ef migrations add NomeDaMigration -c BooksDbContext -o Migrations
```

### Executar Localmente (sem Docker)

```bash
cd src/backend/WebApi/Onix.Writebook.WebApi
dotnet run
```

## 🐛 Troubleshooting

### API não inicia
1. Verifique se Docker está rodando
2. Verifique se portas 8080 e 5432 estão livres
3. Verifique logs: `docker compose logs writebook-api`

### Migrations não aplicadas
1. Verifique `APPLY_MIGRATIONS=true` no `.env`
2. Veja logs para erros específicos
3. Execute migrations manualmente se necessário

### Build falha
1. Limpe cache: `docker builder prune -f`
2. Tente build limpo: `docker build --no-cache ...`
3. Verifique espaço em disco

## 📚 Documentação

- [`QUICK_START.md`](QUICK_START.md) - Guia de início rápido
- [`PORTAINER_DEPLOYMENT.md`](PORTAINER_DEPLOYMENT.md) - Deploy em produção
- [`.env.example`](.env.example) - Variáveis de ambiente

## 🔗 Links Úteis

- [Documentação .NET 8](https://docs.microsoft.com/dotnet)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Docker](https://docs.docker.com)
- [PostgreSQL](https://www.postgresql.org/docs)

