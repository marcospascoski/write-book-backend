#!/bin/bash

#==============================================================================
# WriteBook API - Instalador Completo v2.1
# Opção 1: Instalação inicial (cria banco automaticamente)
# Opção 2: Atualização da API (rebuild e redeploy)
#==============================================================================

set -e

# Cores
readonly RED='\033[0;31m'
readonly GREEN='\033[0;32m'
readonly YELLOW='\033[1;33m'
readonly BLUE='\033[0;34m'
readonly CYAN='\033[0;36m'
readonly WHITE='\033[1;37m'
readonly GRAY='\033[0;90m'
readonly NC='\033[0m'

# Configurações
readonly ENV_FILE=".env"
readonly DOCKER_IMAGE="writebook-api:latest"
readonly COMPOSE_FILE="docker-compose.writebook.yml"
readonly STACK_NAME="writebook"
readonly BACKUP_DIR="backups"

print_banner() {
    clear
    echo -e "${CYAN}"
    cat << "EOF"
╔════════════════════════════════════════════════════════════════╗
║                                                                ║
║              WriteBook API - Instalador v2.1                   ║
║                                                                ║
╚════════════════════════════════════════════════════════════════╝
EOF
    echo -e "${NC}\n"
}

print_success() { echo -e "${GREEN}✓ $1${NC}"; }
print_error() { echo -e "${RED}✗ $1${NC}"; }
print_warning() { echo -e "${YELLOW}⚠ $1${NC}"; }
print_info() { echo -e "${BLUE}ℹ $1${NC}"; }
print_step() { echo -e "${CYAN}➤ $1${NC}"; }
pause() { echo ""; read -p "$(echo -e ${GRAY}Pressione ENTER...${NC})"; }
confirm() { local r; read -p "$(echo -e ${YELLOW}$1? [S/n]: ${NC})" r; [[ ! $r =~ ^[Nn]$ ]]; }

# Desabilitar migrations no .env
disable_migrations() {
    if [ -f "$ENV_FILE" ]; then
        print_step "Desabilitando migrations automáticas..."
        sed -i 's/APPLY_MIGRATIONS=true/APPLY_MIGRATIONS=false/g' "$ENV_FILE"
        print_success "APPLY_MIGRATIONS alterado para false"
        return 0
    fi
    return 1
}

# Habilitar migrations no .env
enable_migrations() {
    if [ -f "$ENV_FILE" ]; then
        print_step "Habilitando migrations temporariamente..."
        sed -i 's/APPLY_MIGRATIONS=false/APPLY_MIGRATIONS=true/g' "$ENV_FILE"
        print_success "APPLY_MIGRATIONS alterado para true"
        return 0
    fi
    return 1
}

# Aguardar aplicação das migrations
wait_for_migrations() {
    print_step "Aguardando aplicação das migrations..."
    
    local max_wait=180  # Aumentado para 3 minutos
    local elapsed=0
    local interval=5
    
    # Verificar se o container está rodando
    if ! docker compose -f $COMPOSE_FILE ps writebook-api 2>/dev/null | grep -q "Up"; then
        print_error "Container writebook-api não está rodando!"
        return 1
    fi
    
    while [ $elapsed -lt $max_wait ]; do
        # Verificar logs do container (últimas 100 linhas para garantir)
        local logs=$(docker compose -f $COMPOSE_FILE logs writebook-api 2>/dev/null | tail -100)
        
        # Buscar mensagem de sucesso (case-insensitive)
        if echo "$logs" | grep -iq "Todas as migrations aplicadas com sucesso"; then
            echo ""
            print_success "Migrations aplicadas com sucesso!"
            return 0
        fi
        
        # Buscar mensagem de erro
        if echo "$logs" | grep -iq "Erro ao aplicar migrations"; then
            echo ""
            print_error "Erro ao aplicar migrations!"
            echo ""
            print_info "Últimas linhas do log:"
            echo "$logs" | tail -20
            return 1
        fi
        
        # Verificar se migrations foram puladas
        if echo "$logs" | grep -iq "APPLY_MIGRATIONS não está habilitado"; then
            echo ""
            print_warning "APPLY_MIGRATIONS está desabilitado no .env"
            return 1
        fi
        
        # Mostrar progresso
        echo -ne "\r${GRAY}Aguardando migrations... ${elapsed}s / ${max_wait}s${NC}"
        sleep $interval
        elapsed=$((elapsed + interval))
    done
    
    echo ""
    print_warning "Timeout ao aguardar migrations (${max_wait}s)"
    print_info "Verifique os logs manualmente:"
    print_info "  docker compose -f $COMPOSE_FILE logs writebook-api | tail -50"
    return 1
}

# Verificar se banco foi criado
check_database() {
    print_step "Verificando banco de dados..."
    
    local postgres_container=$(docker ps -q -f name=postgres)
    if [ -z "$postgres_container" ]; then
        return 1
    fi
    
    # Verificar se o banco tem tabelas
    local tables=$(docker exec $postgres_container psql -U postgres -d db-write-book -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public';" 2>/dev/null | tr -d ' ')
    
    if [ "$tables" -gt 0 ]; then
        print_success "Banco de dados criado com $tables tabelas"
        return 0
    fi
    
    return 1
}

# Verificar e clonar Onix.Framework
check_onix_framework() {
    print_step "Verificando Onix.Framework..."
    
    if [ -d "Onix.Framework" ]; then
        if [ -d "Onix.Framework/.git" ]; then
            print_success "Onix.Framework encontrado"
            
            # Verificar se precisa atualizar
            if confirm "Deseja atualizar o Onix.Framework"; then
                print_info "Atualizando Onix.Framework..."
                (cd Onix.Framework && git pull)
                
                if [ $? -eq 0 ]; then
                    print_success "Onix.Framework atualizado!"
                else
                    print_warning "Erro ao atualizar, continuando com versão atual"
                fi
            fi
            return 0
        else
            print_warning "Diretório Onix.Framework existe mas não é um repositório Git"
            if confirm "Deseja remover e clonar novamente"; then
                rm -rf Onix.Framework
            else
                print_error "Onix.Framework inválido. Abortando."
                return 1
            fi
        fi
    fi
    
    # Clonar Onix.Framework
    print_info "Clonando Onix.Framework..."
    print_info "Repositório: https://github.com/marcospascoski/Onix.Framework"
    
    git clone https://github.com/marcospascoski/Onix.Framework.git Onix.Framework
    
    if [ $? -eq 0 ]; then
        print_success "Onix.Framework clonado com sucesso!"
        return 0
    else
        print_error "Erro ao clonar Onix.Framework!"
        print_error "Verifique sua conexão e tente novamente"
        return 1
    fi
}


# OPÇÃO 1: Instalação Inicial Completa
wizard_install() {
    print_banner
    echo -e "${WHITE}═══════════════════════════════════════════════════${NC}"
    echo -e "${WHITE}  INSTALAÇÃO INICIAL - WIZARD AUTOMÁTICO${NC}"
    echo -e "${WHITE}═══════════════════════════════════════════════════${NC}"
    echo ""
    
    print_info "Este wizard irá:"
    echo "  1. Verificar pré-requisitos e Onix.Framework"
    echo "  2. Configurar ambiente (.env)"
    echo "  3. Build da imagem Docker"
    echo "  4. Deploy e criação do banco"
    echo "  5. Desabilitar migrations automáticas"
    echo ""
    
    if ! confirm "Deseja continuar"; then
        return 0
    fi
    
    # Passo 1: Pré-requisitos
    print_banner
    print_step "PASSO 1/5: Verificando pré-requisitos..."
    echo ""
    
    command -v docker &> /dev/null && print_success "Docker OK" || { print_error "Docker não encontrado!"; pause; return 1; }
    docker compose version &> /dev/null && print_success "Docker Compose OK" || { print_error "Compose não encontrado!"; pause; return 1; }
    command -v git &> /dev/null && print_success "Git OK" || { print_error "Git não encontrado!"; pause; return 1; }
    [ -d "src/backend" ] && print_success "Estrutura OK" || { print_error "Execute na raiz!"; pause; return 1; }
    
    echo ""
    
    # Verificar/clonar Onix.Framework
    if ! check_onix_framework; then
        pause
        return 1
    fi
    
    pause
    
    # Passo 2: Configurar .env
    print_banner
    print_step "PASSO 2/5: Configurando ambiente..."
    echo ""
    
    if [ -f "$ENV_FILE" ]; then
        print_warning "Arquivo .env já existe!"
        if ! confirm "Deseja reconfigurar"; then
            pause
        else
            # Backup
            cp "$ENV_FILE" ".env.backup.$(date +%Y%m%d_%H%M%S)"
            rm "$ENV_FILE"
        fi
    fi
    
    if [ ! -f "$ENV_FILE" ]; then
        read -sp "Senha PostgreSQL [postgres]: " db_pass; echo; db_pass=${db_pass:-postgres}
        read -sp "JWT Secret [gerar auto]: " jwt; echo
        [ -z "$jwt" ] && jwt=$(openssl rand -base64 64 2>/dev/null || cat /dev/urandom | tr -dc 'a-zA-Z0-9' | fold -w 64 | head -n 1)
        
        cat > "$ENV_FILE" << EOF
# WriteBook API - Configuração
# Gerado em: $(date '+%d/%m/%Y %H:%M:%S')

DB_PASSWORD=$db_pass
JWT_SECRET=$jwt
APPLY_MIGRATIONS=true
EOF
        print_success ".env criado com APPLY_MIGRATIONS=true"
    fi
    
    pause
    
    # Passo 3: Build
    print_banner
    print_step "PASSO 3/5: Build da imagem Docker..."
    echo ""
    
    docker builder prune -f
    docker build --no-cache -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t $DOCKER_IMAGE .
    
    if [ $? -ne 0 ]; then
        print_error "Build falhou!"
        pause
        return 1
    fi
    
    print_success "Build concluído!"
    pause
    
    # Passo 4: Deploy e aguardar migrations
    print_banner
    print_step "PASSO 4/5: Deploy e criação do banco..."
    echo ""
    
    print_info "Iniciando serviços..."
    docker compose -f $COMPOSE_FILE up -d
    
    if [ $? -ne 0 ]; then
        print_error "Deploy falhou!"
        pause
        return 1
    fi
    
    print_success "Serviços iniciados!"
    echo ""
    
    # Aguardar migrations
    if wait_for_migrations; then
        sleep 2
        check_database
    else
        print_warning "Não foi possível confirmar se as migrations foram aplicadas"
        print_info "Verifique os logs: docker compose -f $COMPOSE_FILE logs writebook-api"
        
        if ! confirm "Deseja continuar mesmo assim"; then
            return 1
        fi
    fi
    
    pause
    
    # Passo 5: Desabilitar migrations e reiniciar
    print_banner
    print_step "PASSO 5/5: Finalizando configuração..."
    echo ""
    
    # Desabilitar migrations
    if disable_migrations; then
        print_info "Reiniciando serviços com nova configuração..."
        docker compose -f $COMPOSE_FILE restart
        
        if [ $? -eq 0 ]; then
            print_success "Serviços reiniciados!"
        fi
    fi
    
    # Finalização
    print_banner
    echo -e "${GREEN}"
    echo "╔═══════════════════════════════════════════════════════════╗"
    echo "║                                                           ║"
    echo "║        ✓ INSTALAÇÃO CONCLUÍDA COM SUCESSO! ✓            ║"
    echo "║                                                           ║"
    echo "╚═══════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    echo ""
    print_info "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    print_info "  API:     http://localhost:8080"
    print_info "  Swagger: http://localhost:8080/swagger"
    print_info "  Banco:   localhost:5432"
    print_info "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    echo ""
    print_success "✓ Banco de dados criado automaticamente"
    print_success "✓ Migrations desabilitadas para próximas inicializações"
    echo ""
    print_info "Para atualizações futuras, use a Opção 2 do menu."
}

# OPÇÃO 2: Atualização da API
update_api() {
    print_banner
    echo -e "${WHITE}═══════════════════════════════════════════════════${NC}"
    echo -e "${WHITE}  ATUALIZAÇÃO DA API${NC}"
    echo -e "${WHITE}═══════════════════════════════════════════════════${NC}"
    echo ""
    
    print_info "Este processo irá:"
    echo "  1. (Opcional) Atualizar código do Git"
    echo "  2. Fazer backup do banco atual"
    echo "  3. Rebuild da imagem"
    echo "  4. Redeploy da aplicação"
    echo "  5. (Opcional) Executar novas migrations"
    echo ""
    
    if ! confirm "Deseja continuar"; then
        return 0
    fi
    
    # Passo 1: Git Pull (opcional)
    print_banner
    print_step "PASSO 1/5: Atualizar código do repositório"
    echo ""
    
    if [ -d ".git" ]; then
        if confirm "Deseja fazer git pull"; then
            print_info "Atualizando código..."
            git pull
            print_success "Código atualizado!"
        fi
    else
        print_info "Não é um repositório Git, pulando..."
    fi
    
    echo ""
    
    # Verificar/atualizar Onix.Framework
    if ! check_onix_framework; then
        pause
        return 1
    fi
    
    pause
    
    # Passo 2: Backup
    print_banner
    print_step "PASSO 2/5: Backup do banco de dados"
    echo ""
    
    if confirm "Deseja fazer backup do banco antes da atualização"; then
        mkdir -p "$BACKUP_DIR"
        local postgres_container=$(docker ps -q -f name=postgres)
        
        if [ -n "$postgres_container" ]; then
            local backup_file="$BACKUP_DIR/backup_pre_update_$(date +%Y%m%d_%H%M%S).sql"
            print_info "Criando backup..."
            
            docker exec $postgres_container pg_dump -U postgres db-write-book > "$backup_file"
            
            if [ $? -eq 0 ]; then
                local size=$(du -h "$backup_file" | cut -f1)
                print_success "Backup criado: $backup_file ($size)"
            else
                print_error "Erro ao criar backup!"
                if ! confirm "Deseja continuar sem backup"; then
                    return 1
                fi
            fi
        else
            print_warning "Container PostgreSQL não encontrado"
        fi
    fi
    
    pause
    
    # Passo 3: Rebuild
    print_banner
    print_step "PASSO 3/5: Rebuild da imagem"
    echo ""
    
    print_info "Limpando cache..."
    docker builder prune -f
    
    print_info "Iniciando build..."
    docker build -f src/backend/WebApi/Onix.Writebook.WebApi/Dockerfile -t $DOCKER_IMAGE .
    
    if [ $? -ne 0 ]; then
        print_error "Build falhou!"
        pause
        return 1
    fi
    
    print_success "Build concluído!"
    pause
    
    # Passo 4: Migrations (se necessário)
    print_banner
    print_step "PASSO 4/5: Migrations"
    echo ""
    
    print_info "Esta atualização inclui novas migrations?"
    if confirm "Executar migrations"; then
        print_info "Habilitando migrations temporariamente..."
        enable_migrations
        
        print_info "As migrations serão executadas no próximo restart"
        local run_migrations=true
    else
        print_info "Migrations não serão executadas"
        local run_migrations=false
    fi
    
    pause
    
    # Passo 5: Redeploy
    print_banner
    print_step "PASSO 5/5: Redeploy da aplicação"
    echo ""
    
    if docker info | grep -q "Swarm: active"; then
        print_info "Atualizando stack Swarm..."
        docker service update --image $DOCKER_IMAGE ${STACK_NAME}_writebook-api
        
        if [ $? -eq 0 ]; then
            print_success "Serviço atualizado!"
            
            if [ "$run_migrations" = true ]; then
                print_info "Aguardando migrations..."
                sleep 10
                
                # Desabilitar migrations novamente
                disable_migrations
                docker service update --force ${STACK_NAME}_writebook-api
            fi
        fi
    else
        print_info "Reiniciando serviços Docker Compose..."
        docker compose -f $COMPOSE_FILE up -d --force-recreate
        
        if [ $? -eq 0 ]; then
            print_success "Serviços atualizados!"
            
            if [ "$run_migrations" = true ]; then
                wait_for_migrations
                
                # Desabilitar migrations novamente
                disable_migrations
                docker compose -f $COMPOSE_FILE restart
            fi
        fi
    fi
    
    # Finalização
    print_banner
    echo -e "${GREEN}"
    echo "╔═══════════════════════════════════════════════════════════╗"
    echo "║                                                           ║"
    echo "║        ✓ ATUALIZAÇÃO CONCLUÍDA COM SUCESSO! ✓           ║"
    echo "║                                                           ║"
    echo "╚═══════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    echo ""
    print_success "API atualizada e rodando!"
    
    if [ "$run_migrations" = true ]; then
        print_success "Migrations executadas e desabilitadas automaticamente"
    fi
}

# MENU PRINCIPAL
main_menu() {
    while true; do
        print_banner
        
        # Status
        if docker ps -q -f name=writebook-api &> /dev/null; then
            echo -e "${GREEN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
            echo -e "${GREEN}  ✓ API em execução - http://localhost:8080${NC}"
            echo -e "${GREEN}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
            echo ""
        fi
        
        echo -e "${WHITE}╔════════════════════════════════════════════════════╗${NC}"
        echo -e "${WHITE}║  MENU PRINCIPAL                                    ║${NC}"
        echo -e "${WHITE}╚════════════════════════════════════════════════════╝${NC}"
        echo ""
        
        echo -e "  ${WHITE}1.${NC} 🚀 Instalação Inicial Completa ${GREEN}(Primeira vez)${NC}"
        echo -e "  ${WHITE}2.${NC} 🔄 Atualizar API ${CYAN}(Nova versão)${NC}"
        echo ""
        echo -e "${GRAY}  ───────────────────────────────────────────────────${NC}"
        echo -e "  ${WHITE}3.${NC} 📋 Ver logs"
        echo -e "  ${WHITE}4.${NC} 💾 Backup do banco"
        echo -e "  ${WHITE}5.${NC} 📥 Restaurar banco"
        echo -e "  ${WHITE}6.${NC} 🗑️  Limpar cache Docker"
        echo -e "  ${WHITE}7.${NC} 🔧 Gerenciar serviços"
        echo ""
        echo -e "${GRAY}  ───────────────────────────────────────────────────${NC}"
        echo -e "  ${WHITE}0.${NC} Sair"
        echo ""
        
        read -p "Opção: " choice
        
        case $choice in
            1) wizard_install; pause ;;
            2) update_api; pause ;;
            3) 
                print_info "Logs em tempo real (Ctrl+C para sair)"
                sleep 1
                docker compose -f $COMPOSE_FILE logs -f
                ;;
            4)
                mkdir -p "$BACKUP_DIR"
                local file="$BACKUP_DIR/backup_$(date +%Y%m%d_%H%M%S).sql"
                docker exec $(docker ps -q -f name=postgres) pg_dump -U postgres db-write-book > "$file"
                print_success "Backup: $file"
                pause
                ;;
            5)
                ls -lh $BACKUP_DIR/*.sql 2>/dev/null || { print_warning "Sem backups"; pause; continue; }
                echo ""
                read -p "Arquivo: " file
                cat "$file" | docker exec -i $(docker ps -q -f name=postgres) psql -U postgres db-write-book
                print_success "Restaurado!"
                pause
                ;;
            6)
                docker builder prune -f
                print_success "Cache limpo!"
                pause
                ;;
            7)
                echo ""
                echo "1. Iniciar  2. Parar  3. Reiniciar"
                read -p "Opção: " s
                case $s in
                    1) docker compose -f $COMPOSE_FILE up -d ;;
                    2) docker compose -f $COMPOSE_FILE down ;;
                    3) docker compose -f $COMPOSE_FILE restart ;;
                esac
                pause
                ;;
            0)
                print_success "Até logo!"
                exit 0
                ;;
            *)
                print_error "Opção inválida!"
                pause
                ;;
        esac
    done
}

# Verificação inicial
[ ! -d "src/backend" ] && { print_error "Execute na raiz do projeto!"; exit 1; }

# Executar
main_menu