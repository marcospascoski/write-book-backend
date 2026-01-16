# WriteBook API - Stack Portainer

Esta stack inclui a API WriteBook com PostgreSQL e aplicação automática de migrations.

## 📋 Pré-requisitos

- Docker Swarm inicializado
- Portainer instalado
- Imagem `writebook-api:latest` construída e disponível

## 🏗️ Build da Imagem

Execute na **raiz do repositório** (`D:\Dev\write-book-backend`):

```bash
docker build -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t writebook-api:latest .
```

## 🚀 Deploy no Portainer

### 1. Acesse o Portainer
- Vá para **Stacks** → **Add Stack**

### 2. Configure a Stack
- **Name**: `writebook`
- **Build method**: Git Repository ou Web editor
- Cole o conteúdo do arquivo `docker-compose.writebook.yml`

### 3. Variáveis de Ambiente

Adicione as seguintes variáveis de ambiente no Portainer:

| Variável | Descrição | Valor Padrão | Obrigatório |
|----------|-----------|--------------|-------------|
| `DB_PASSWORD` | Senha do PostgreSQL | `postgres` | Não |
| `JWT_SECRET` | Chave secreta para JWT | `your-secret-key-change-in-production` | **Sim** (em produção) |

**⚠️ IMPORTANTE**: Em produção, sempre defina um `JWT_SECRET` forte!

### 4. Deploy

Clique em **Deploy the stack**

## 🔄 Migrations Automáticas

As migrations são aplicadas automaticamente na primeira inicialização através da variável `APPLY_MIGRATIONS=true`.

### Contextos de Banco de Dados

A aplicação possui 3 DbContexts:

1. **SistemaDbContext** - Configurações do sistema
2. **AcessosDbContext** - Autenticação e autorização
3. **BooksDbContext** - Gerenciamento de livros

Todas as migrations são aplicadas automaticamente ao iniciar o container.

## 📊 Verificação

Após o deploy:

1. **Verifique os logs**:
   ```bash
   docker service logs writebook_writebook-api -f
   ```

2. **Acesse a API**:
   - Swagger: http://localhost:8080/swagger

3. **Verifique o banco de dados**:
   ```bash
   docker exec -it $(docker ps -q -f name=writebook_postgres) psql -U postgres -d db-write-book
   ```
   
   No PostgreSQL:
   ```sql
   \dt  -- Lista todas as tabelas
   ```

## 🔧 Comandos Úteis

### Rebuild e Redeploy
```bash
# Build
docker build -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t writebook-api:latest .

# Tag para registry (opcional)
docker tag writebook-api:latest seu-registry/writebook-api:latest
docker push seu-registry/writebook-api:latest

# Update no Portainer ou via CLI
docker service update --image writebook-api:latest writebook_writebook-api
```

### Ver Logs
```bash
# API
docker service logs writebook_writebook-api -f

# PostgreSQL
docker service logs writebook_postgres -f
```

### Escalar Serviço
```bash
docker service scale writebook_writebook-api=3
```

### Remover Stack
No Portainer: **Stacks** → Selecione `writebook` → **Delete**

Ou via CLI:
```bash
docker stack rm writebook
```

## 🔐 Segurança em Produção

### Checklist:
- [ ] Alterar `DB_PASSWORD` para senha forte
- [ ] Definir `JWT_SECRET` com valor criptograficamente seguro
- [ ] Configurar HTTPS/TLS
- [ ] Restringir portas expostas (usar reverse proxy)
- [ ] Implementar rate limiting
- [ ] Configurar backups do PostgreSQL
- [ ] Revisar políticas CORS

### Exemplo de JWT_SECRET Seguro
```bash
# Gerar secret aleatório (Linux/Mac)
openssl rand -base64 64

# PowerShell
[Convert]::ToBase64String((1..64 | ForEach-Object { Get-Random -Maximum 256 }))
```

## 💾 Backup do Banco de Dados

### Backup Manual
```bash
docker exec $(docker ps -q -f name=writebook_postgres) \
  pg_dump -U postgres db-write-book > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restore
```bash
cat backup_20240101_120000.sql | \
  docker exec -i $(docker ps -q -f name=writebook_postgres) \
  psql -U postgres db-write-book
```

## 🐛 Troubleshooting

### Migrations não aplicadas
1. Verifique os logs: `docker service logs writebook_writebook-api`
2. Verifique se `APPLY_MIGRATIONS=true`
3. Conecte manualmente ao container e execute:
   ```bash
   docker exec -it $(docker ps -q -f name=writebook_writebook-api) /bin/bash
   dotnet ef database update --context SistemaDbContext
   dotnet ef database update --context AcessosDbContext
   dotnet ef database update --context BooksDbContext
   ```

### Container não inicia
1. Verifique se o PostgreSQL está saudável: `docker service ps writebook_postgres`
2. Verifique connection string nas variáveis de ambiente
3. Revise logs de ambos os serviços

### Problemas de permissão
Se encontrar problemas com o usuário `appuser`, ajuste as permissões no Dockerfile ou execute como root (não recomendado em produção).

## 📝 Notas

- A stack usa **overlay network** para comunicação entre serviços
- O volume `postgres_data` persiste os dados do banco
- Migrations são **idempotentes** - seguro executar múltiplas vezes
- Deploy configurado para **zero-downtime** com `order: start-first`

## 🤝 Suporte

Para problemas ou dúvidas:
1. Verifique os logs
2. Consulte a documentação do Entity Framework Core
3. Abra uma issue no repositório
