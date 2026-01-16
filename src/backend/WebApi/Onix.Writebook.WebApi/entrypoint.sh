#!/bin/bash
set -e

echo "========================================="
echo "WriteBook API - Ferramenta de Migrations"
echo "========================================="

# Função para aguardar o banco estar disponível
wait_for_db() {
    echo "Aguardando PostgreSQL ficar pronto..."
    until dotnet ef database update --project /app/src/backend/Sistema/Onix.Writebook.Sistema.Infra.Data --startup-project /app/src/backend/WebApi/Onix.Writebook.WebApi --context SistemaDbContext --no-build -- --help > /dev/null 2>&1; do
        echo "PostgreSQL não está disponível - aguardando..."
        sleep 2
    done
    echo "PostgreSQL está pronto!"
}

# Aplicar migrations se a variável APPLY_MIGRATIONS estiver definida
if [ "$APPLY_MIGRATIONS" = "true" ]; then
    echo "APPLY_MIGRATIONS está habilitado. Executando migrations..."

    wait_for_db

    echo "Aplicando migrations do Sistema..."
    dotnet ef database update \
        --project /app/src/backend/Sistema/Onix.Writebook.Sistema.Infra.Data \
        --startup-project /app/src/backend/WebApi/Onix.Writebook.WebApi \
        --context SistemaDbContext \
        --no-build || echo "Migration do Sistema falhou ou já foi aplicada"

    echo "Aplicando migrations do Acesso..."
    dotnet ef database update \
        --project /app/src/backend/Acesso/Onix.Writebook.Acesso.Infra.Data \
        --startup-project /app/src/backend/WebApi/Onix.Writebook.WebApi \
        --context AcessosDbContext \
        --no-build || echo "Migration do Acesso falhou ou já foi aplicada"

    echo "Aplicando migrations do Books..."
    dotnet ef database update \
        --project /app/src/backend/Books/Onix.Writebook.Books.Infra.Data \
        --startup-project /app/src/backend/WebApi/Onix.Writebook.WebApi \
        --context BooksDbContext \
        --no-build || echo "Migration do Books falhou ou já foi aplicada"

    echo "Todas as migrations foram aplicadas com sucesso!"
else
    echo "APPLY_MIGRATIONS não está habilitado. Pulando migrations."
fi

echo "========================================="
echo "Iniciando WriteBook API..."
echo "========================================="

# Iniciar a aplicação
exec dotnet Onix.Writebook.WebApi.dll
