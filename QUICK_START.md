# 🚀 WriteBook API - Guia de Instalação Rápida

## ⚡ Início Rápido - 3 Passos

### 1️⃣ Build da Imagem

```bash
cd D:\Dev\write-book-backend
docker build -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t writebook-api:latest .
```

### 2️⃣ Configurar Ambiente

Copie o arquivo de exemplo:
```bash
cp .env.example .env
```

Edite o `.env` e configure:
- `DB_PASSWORD`: Senha do PostgreSQL
- `JWT_SECRET`: Chave secreta JWT (ou deixe gerar automaticamente)
- `APPLY_MIGRATIONS`: `true` para primeira instalação

### 3️⃣ Iniciar Serviços

```bash
docker compose -f docker-compose.writebook.yml up -d
```

## 🎯 Acessar a API

- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Banco**: localhost:5432

## 📋 Comandos Úteis

### Ver Logs
```bash
docker compose -f docker-compose.writebook.yml logs -f
```

### Parar Serviços
```bash
docker compose -f docker-compose.writebook.yml down
```

### Reiniciar Serviços
```bash
docker compose -f docker-compose.writebook.yml restart
```

### Backup do Banco
```bash
mkdir -p backups
docker exec $(docker ps -q -f name=postgres) pg_dump -U postgres db-write-book > backups/backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restaurar Banco
```bash
cat backups/backup_20240101_120000.sql | docker exec -i $(docker ps -q -f name=postgres) psql -U postgres db-write-book
```

## 🔧 Instalador Interativo

Para instalação guiada completa com wizard:

```bash
./instalador.sh
```

**Opções do Menu:**
1. 🚀 **Instalação Inicial Completa** - Setup automático (recomendado primeira vez)
   - Verifica pré-requisitos
   - Configura .env
   - Build da imagem
   - Deploy e criação do banco
   - Desabilita migrations automaticamente após criação
   
2. 🔄 **Atualizar API** - Update para novas versões
   - Git pull (opcional)
   - Backup automático do banco
   - Rebuild da imagem
   - Pergunta sobre novas migrations
   - Redeploy com zero-downtime
   
3. 📋 Ver logs em tempo real
4. 💾 Backup do banco de dados
5. 📥 Restaurar banco de dados
6. 🗑️ Limpar cache Docker
7. 🔧 Gerenciar serviços (iniciar/parar/reiniciar)

## 🐛 Troubleshooting

### Migrations não aplicadas?
```bash
# Ver logs da API
docker compose -f docker-compose.writebook.yml logs writebook-api

# Verificar variável de ambiente
cat .env | grep APPLY_MIGRATIONS

# Forçar aplicação de migrations
docker compose -f docker-compose.writebook.yml down
# Altere APPLY_MIGRATIONS=true no .env
docker compose -f docker-compose.writebook.yml up -d
```

### API não inicia?
```bash
# Verificar status dos containers
docker compose -f docker-compose.writebook.yml ps

# Verificar se PostgreSQL está saudável
docker ps

# Ver logs de erro
docker compose -f docker-compose.writebook.yml logs

# Reiniciar serviços
docker compose -f docker-compose.writebook.yml restart
```

### Porta 8080 já está em uso?
```bash
# Windows (PowerShell)
Get-Process -Id (Get-NetTCPConnection -LocalPort 8080).OwningProcess

# Mudar porta no docker-compose.writebook.yml:
# ports:
#   - "8081:8080"  # Muda para 8081
```

### Limpar tudo e começar do zero
```bash
# ATENÇÃO: Isso remove TODOS os dados!
docker compose -f docker-compose.writebook.yml down -v
rm .env
docker system prune -a -f

# Depois refaça os passos 1, 2 e 3
```

### Build falha?
```bash
# Limpar cache do Docker
docker builder prune -f

# Build sem cache
docker build --no-cache -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t writebook-api:latest .
```

## 🔐 Segurança em Produção

⚠️ **IMPORTANTE antes de ir para produção:**

1. **Gere JWT Secret forte:**
   ```bash
   # Linux/Mac
   openssl rand -base64 64
   
   # PowerShell
   [Convert]::ToBase64String((1..64|ForEach-Object{Get-Random -Maximum 256}))
   ```

2. **Use senha forte no PostgreSQL** (mínimo 12 caracteres)

3. **Configure HTTPS/TLS** com certificados válidos

4. **Revise políticas CORS** em `ApiConfig.cs`

5. **Configure backups automáticos**

6. **Após primeira instalação, altere:**
   ```
   APPLY_MIGRATIONS=false
   ```

7. **Proteja o arquivo .env** (nunca commitar no Git)

8. **Use variáveis de ambiente do sistema** para produção

## 📚 Documentação Completa

Para informações detalhadas:
- `PORTAINER_DEPLOYMENT.md` - Deploy em Portainer/Swarm
- `instalador.sh` - Script de instalação automatizado
- `.env.example` - Todas as variáveis disponíveis

## 🔄 Workflow de Atualização

### 1. Fazer backup
```bash
./instalador.sh
# Escolha opção 4 (Backup do banco)
```

### 2. Atualizar código
```bash
git pull
cd Onix.Framework
git pull
cd ..
```

### 3. Rebuild e redeploy
```bash
./instalador.sh
# Escolha opção 2 (Atualizar API)
```

## 💡 Dicas Úteis

### Ver tabelas criadas no banco
```bash
docker exec -it $(docker ps -q -f name=postgres) psql -U postgres db-write-book
\dt
\q
```

### Monitorar uso de recursos
```bash
docker stats
```

### Ver histórico de migrations
```bash
docker exec $(docker ps -q -f name=postgres) psql -U postgres db-write-book -c "SELECT * FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\";"
```

## 🆘 Suporte

**Problemas comuns:**
1. ✅ Docker está rodando?
2. ✅ Porta 8080 está livre?
3. ✅ Porta 5432 está livre?
4. ✅ `.env` está configurado corretamente?
5. ✅ Estrutura de pastas está correta?

**Para mais ajuda:**
- Veja os logs: `docker compose -f docker-compose.writebook.yml logs`
- Use o instalador interativo: `./instalador.sh`
- Consulte: `PORTAINER_DEPLOYMENT.md`

## 📁 Estrutura de Arquivos

```
write-book-backend/
├── instalador.sh                     ← Execute para instalação guiada
├── docker-compose.writebook.yml      ← Configuração Docker Compose
├── .env.example                      ← Template de configuração
├── .env                              ← Suas configurações (NÃO COMMITAR)
├── backups/                          ← Backups do banco
├── src/backend/                      ← Código fonte
└── Onix.Framework/                   ← Framework base
```

---

**Feito com ❤️ pelo WriteBook Team**

**Versão:** 2.1 | **Última atualização:** 2024
