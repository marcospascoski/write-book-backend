# Guia de Testes - Autenticação e Permissões

Este guia explica como testar o sistema de autenticação, login, logout, perfis e permissões.

## ?? Índice

1. [Testes de Autenticação](#testes-de-autenticação)
2. [Testes de Perfil](#testes-de-perfil)
3. [Testes de Permissões](#testes-de-permissões)
4. [Como Executar os Testes](#como-executar-os-testes)
5. [Testes de Integração via API](#testes-de-integração-via-api)

---

## ?? Testes de Autenticação

### Arquivo: `AuthAppServiceTests.cs`

#### Cenários de Login

1. **Login com sucesso**
   - ? Usuário válido com credenciais corretas
   - ? Retorna AccessToken
   - ? Senha não é retornada no response

2. **Falhas de Login**
   - ? Email não encontrado
   - ? Senha inválida
   - ? Email vazio
   - ? Senha vazia

3. **Logout**
   - ? Logout com sucesso
   - ? Token bloqueado após logout (blacklist)

### Exemplo de Uso:

```csharp
// Login bem-sucedido
var loginViewModel = new LoginViewModel
{
    Email = "usuario@exemplo.com",
    Senha = "senha123"
};

var result = await _authAppService.Login(loginViewModel);
Assert.NotNull(result.AccessToken);
```

---

## ?? Testes de Perfil

### Arquivo: `PerfilAppServiceTests.cs`

#### Operações CRUD

1. **Cadastrar Perfil**
   - ? Criar perfil válido
   - ? Criar perfil sem nome
   - ? Criar perfil duplicado

2. **Alterar Perfil**
   - ? Alterar dados do perfil
   - ? Alterar perfil inexistente

3. **Alterar Status**
   - ? Ativar perfil
   - ? Inativar perfil

4. **Pesquisar Perfil**
   - ? Buscar por ID
   - ? Paginação de perfis
   - ? Filtrar por texto

### Exemplo de Uso:

```csharp
// Criar novo perfil
var perfilViewModel = new PerfilViewModel
{
    Nome = "Administrador",
    Descricao = "Perfil de administrador do sistema"
};

var perfilId = await _perfilAppService.Cadastrar(perfilViewModel);
Assert.True(perfilId > 0);
```

---

## ?? Testes de Permissões

### Arquivo: `PerfilPermissaoAppServiceTests.cs`

#### Vinculação Perfil-Permissão

1. **Cadastrar Relação**
   - ? Vincular permissão a perfil
   - ? Vincular permissão duplicada
   - ? Perfil inexistente
   - ? Permissão inexistente

2. **Remover Relação**
   - ? Remover permissão de perfil
   - ? Remover relação inexistente

3. **Cenários Avançados**
   - ? Múltiplas permissões para um perfil
   - ? Mesma permissão em perfis diferentes

### Exemplo de Uso:

```csharp
// Vincular permissão a perfil
var perfilPermissaoViewModel = new PerfilPermissaoViewModel
{
    PerfilId = 1,
    PermissaoId = 5
};

var resultado = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel);
Assert.True(resultado > 0);
```

---

## ?? Como Executar os Testes

### Via Visual Studio

1. Abra o **Test Explorer**:
   - Menu: `Test > Test Explorer`
   - Atalho: `Ctrl + E, T`

2. Execute os testes:
   - **Todos os testes**: Clique em "Run All"
   - **Teste específico**: Clique com botão direito no teste > "Run"
   - **Classe específica**: Clique com botão direito na classe > "Run"

### Via Linha de Comando

```bash
# Executar todos os testes do projeto Acesso
dotnet test Acesso\Onix.Writebook.Acesso.Tests\Onix.Writebook.Acesso.Tests.csproj

# Executar apenas testes de autenticação
dotnet test --filter "FullyQualifiedName~AuthAppServiceTests"

# Executar apenas testes de perfil
dotnet test --filter "FullyQualifiedName~PerfilAppServiceTests"

# Executar apenas testes de permissão
dotnet test --filter "FullyQualifiedName~PerfilPermissaoAppServiceTests"

# Executar com detalhamento
dotnet test --logger "console;verbosity=detailed"
```

### Executar por Categoria

```bash
# Se os testes tiverem traits/categories
dotnet test --filter "Category=Authentication"
dotnet test --filter "Category=Authorization"
```

---

## ?? Testes de Integração via API

### Usando Postman/Insomnia

#### 1. Registrar Usuário

```http
POST /api/auth/register
Content-Type: application/json

{
  "nome": "João Silva",
  "email": "joao@exemplo.com",
  "password": "senha123",
  "referralCode": "ABC123"
}
```

#### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "joao@exemplo.com",
  "senha": "senha123"
}
```

**Response:**
```json
{
  "id": "guid-do-usuario",
  "nome": "João Silva",
  "email": "joao@exemplo.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "balance": 0,
  "earnedBalance": 0,
  "pendingBalance": 0
}
```

#### 3. Criar Perfil (Requer Autenticação)

```http
POST /api/perfil
Content-Type: application/json
Authorization: Bearer {accessToken}

{
  "nome": "Gerente",
  "descricao": "Perfil de gerente do sistema"
}
```

#### 4. Vincular Permissão a Perfil

```http
POST /api/perfil-permissao
Content-Type: application/json
Authorization: Bearer {accessToken}

{
  "perfilId": 1,
  "permissaoId": 5
}
```

#### 5. Logout

```http
POST /api/auth/logout
Authorization: Bearer {accessToken}
```

---

## ?? Estrutura de Testes

```
Acesso.Tests/
??? Services/
?   ??? AuthAppServiceTests.cs          # Testes de autenticação
?   ??? PerfilAppServiceTests.cs        # Testes de perfis
?   ??? PerfilPermissaoAppServiceTests.cs # Testes de permissões
?   ??? UsuarioServiceTests.cs          # Testes de usuários
??? Moqs/
?   ??? RegistrarUsuarioViewModelMoq.cs # Dados mock para testes
??? Startup.cs                          # Configuração DI para testes
```

---

## ? Checklist de Validação

### Login e Autenticação
- [ ] Login com credenciais válidas
- [ ] Login com email inválido
- [ ] Login com senha inválida
- [ ] Logout funcional
- [ ] Token gerado corretamente
- [ ] Token invalidado após logout

### Perfis
- [ ] Criar perfil
- [ ] Editar perfil
- [ ] Ativar/Inativar perfil
- [ ] Listar perfis
- [ ] Buscar perfil por ID
- [ ] Validar duplicação de nome

### Permissões
- [ ] Vincular permissão a perfil
- [ ] Remover permissão de perfil
- [ ] Validar permissões duplicadas
- [ ] Múltiplas permissões por perfil
- [ ] Mesma permissão em perfis diferentes

---

## ?? Troubleshooting

### Teste Falhando: "Usuario não encontrado"
- Verifique se o usuário foi criado no banco de testes
- Confirme que o email está correto

### Teste Falhando: "Senha inválida"
- Verifique o salt usado na criptografia
- Confirme que a senha foi criptografada corretamente

### Erro de AutoMapper
- Execute: `dotnet test` e verifique as configurações de mapeamento
- Certifique-se de que todos os profiles estão registrados

### Erro de Banco de Dados
- Para testes, use InMemory database
- Limpe o contexto entre os testes com `_acessosUnitOfWork.Untrack<T>(entity)`

---

## ?? Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [AutoMapper](https://automapper.org/)
- [Entity Framework Core InMemory](https://docs.microsoft.com/ef/core/providers/in-memory/)
- [ASP.NET Core Testing](https://docs.microsoft.com/aspnet/core/test/)

---

## ?? Contribuindo

Ao adicionar novos testes:

1. Siga o padrão AAA (Arrange, Act, Assert)
2. Use nomes descritivos (Deve_fazer_algo_quando_condicao)
3. Mantenha os testes independentes
4. Limpe o estado após cada teste
5. Use mocks para dependências externas

---

**Última atualização:** 2024
