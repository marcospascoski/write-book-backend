using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Tests.Moqs;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Core.Resources;
using Xunit;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Enums;

namespace Onix.Writebook.Acesso.Tests.Services
{
    public class UsuarioServiceTests
    (
        INotificationContext notificationContext,
        IUsuarioAppService usuarioService,
        IUsuarioRepository usuarioRepository,
        IUsuarioValidator usuarioValidator,
        IAcessosUnitOfWork acessosUnitOfWork,
        IStringLocalizer<TextResource> stringLocalizer,
        ITokenRedefinicaoSenhaRepository tokenRedefinicaoSenhaRepository)
    {
        private readonly INotificationContext _notificationContext = notificationContext;
        private readonly IUsuarioAppService _usuarioService = usuarioService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioValidator _usuarioValidator = usuarioValidator;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork = acessosUnitOfWork;
        private readonly IStringLocalizer<TextResource> _stringLocalizer = stringLocalizer;
        private readonly ITokenRedefinicaoSenhaRepository _tokenRedefinicaoSenhaRepository = tokenRedefinicaoSenhaRepository;

        [Fact]
        public async Task Deve_criar_usuario_com_sucesso()
        {
            var viewModel = RegistrarUsuarioViewModelMoq.GetUsuarioViewModel();

            var result = await _usuarioService.CadastrarAsync(viewModel);

            Assert.NotEqual(Guid.Empty, result);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_criar_usuario_com_status_pendente_confirmacao()
        {
            var viewModel = RegistrarUsuarioViewModelMoq.GetUsuarioViewModel();

            var usuarioId = await _usuarioService.CadastrarAsync(viewModel);

            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioId);
            Assert.NotNull(usuario);
            Assert.Equal(EStatusUsuario.PendenteConfirmacao, usuario.Status);
        }

        [Fact]
        public async Task Deve_obter_usuario_por_id_com_sucesso()
        {
            var usuario = await CadastrarUsuario();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var result = await _usuarioService.PesquisarPorId(usuario.Id);

            Assert.NotNull(result);
            Assert.Equal(usuario.Id, result.Id);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_usuario_nao_encontrado()
        {
            var usuarioId = Guid.NewGuid();

            var result = await _usuarioService.PesquisarPorId(usuarioId);

            Assert.Null(result);
        }

        [Fact]
        public async Task Deve_obter_usuario_por_email_com_sucesso()
        {
            var email = "usuario@test.com";
            await CadastrarUsuario(email: email);

            var result = await _usuarioService.ObterPorEmailAsync(email);

            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task Deve_retornar_nulo_quando_usuario_nao_existe_por_email()
        {
            var email = "naoexiste@test.com";

            var result = await _usuarioService.ObterPorEmailAsync(email);

            Assert.Null(result);
        }

        [Fact]
        public async Task Deve_validar_usuario_valida()
        {
            var usuario = UsuarioMoq.GetUsuario();

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.True(result);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_alterar_dados_usuario_com_sucesso()
        {
            var usuario = await CadastrarUsuario();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var usuarioAlterado = UsuarioViewModelMoq.GetUsuarioViewModel(
                id: usuario.Id,
                nome: "Novo Nome",
                email: "novoemail@test.com"
            );

            await _usuarioService.Alterar(usuarioAlterado);

            var result = await _usuarioService.PesquisarPorId(usuario.Id);
            Assert.NotNull(result);
            Assert.Equal("Novo Nome", result.Nome);
            Assert.Equal("novoemail@test.com", result.Email);
        }

        [Fact]
        public async Task Deve_contar_usuarios_com_sucesso()
        {
            await CadastrarUsuario();
            await CadastrarUsuario();
            await CadastrarUsuario();

            var count = await _usuarioRepository.CountAsync();

            Assert.True(count >= 3);
        }

        [Fact]
        public async Task Deve_alterar_status_usuario()
        {
            var usuario = await CadastrarUsuario(status: EStatusUsuario.PendenteConfirmacao);
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var statusViewModel = new Application.ViewModels.UsuarioAlterarStatusViewModel 
            { 
                Id = usuario.Id, 
                Status = EStatusUsuario.Ativo.ToString()
            };

            await _usuarioService.AlterarStatus(statusViewModel);

            var result = await _usuarioService.PesquisarPorId(usuario.Id);
            Assert.NotNull(result);
            Assert.Equal((int)EStatusUsuario.Ativo, result.Status);
        }

        [Fact]
        public async Task Deve_pesquisar_usuario_por_email_e_senha()
        {
            var senha = "Senha@123";
            var usuario = await CadastrarUsuario(senha: senha);
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var result = await _usuarioRepository.PesquisarPorEmailESenhaAsync(usuario.Email, senha);

            Assert.NotNull(result);
            Assert.Equal(usuario.Email, result.Email);
        }

        [Fact]
        public async Task Deve_verificar_se_usuario_ja_cadastrado()
        {
            var email = "usuario@test.com";
            var usuario = await CadastrarUsuario(email: email);

            var jaCadastrado = await _usuarioRepository.JaCadastrado(email, usuario.Id);

            Assert.False(jaCadastrado);
        }

        [Fact]
        public async Task Deve_verificar_se_usuario_existe()
        {
            var usuario = await CadastrarUsuario();

            var existe = await _usuarioRepository.Exists(usuario.Id);

            Assert.True(existe);
        }

        [Fact]
        public async Task Deve_solicitar_redefinicao_senha_com_sucesso()
        {
            var email = "usuario@test.com";
            var usuario = await CadastrarUsuario(email: email);
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var solicitarRedefinicaoViewModel = new Application.ViewModels.SolicitarRedefinicaoSenhaViewModel
            {
                Email = email
            };

            var resultado = await _usuarioService.SolicitarRedefinicaoSenhaAsync(solicitarRedefinicaoViewModel);

            Assert.True(resultado);
            Assert.False(_notificationContext.HasErrors);
            
            var token = await _tokenRedefinicaoSenhaRepository.PesquisarTokenValidoPorUsuarioAsync(usuario.Id);
            Assert.NotNull(token);
            Assert.True(token.EstaValido());
        }

        [Fact]
        public async Task Deve_redefinir_senha_com_token_valido()
        {
            var senhaOriginal = "SenhaAntiga@123";
            var novaSenha = "NovaSenha@456";
            var email = "usuario@test.com";
            var usuario = await CadastrarUsuario(email: email, senha: senhaOriginal);
            
            // Cria token de redefinição
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id);
            await _tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
            await _acessosUnitOfWork.CommitAsync();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);
            _acessosUnitOfWork.Untrack<TokenRedefinicaoSenha>(tokenRedefinicaoSenha);

            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = tokenRedefinicaoSenha.Token,
                NovaSenha = novaSenha
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);

            Assert.False(_notificationContext.HasErrors);
            
            var usuarioAutenticado = await _usuarioRepository.PesquisarPorEmailESenhaAsync(email, novaSenha);
            Assert.NotNull(usuarioAutenticado);
            
            var tokenAtualizado = await _tokenRedefinicaoSenhaRepository.PesquisarPorTokenAsync(tokenRedefinicaoSenha.Token);
            Assert.True(tokenAtualizado.Utilizado);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_redefinir_senha_usuario_inexistente()
        {
            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = "token_inexistente",
                NovaSenha = "NovaSenha@123"
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);

            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_autenticar_com_nova_senha_apos_redefinicao()
        {
            var senhaOriginal = "SenhaAntiga@123";
            var novaSenha = "NovaSenha@456";
            var email = "usuario@test.com";
            var usuario = await CadastrarUsuario(email: email, senha: senhaOriginal);
            
            // Cria token de redefinição
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id);
            await _tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
            await _acessosUnitOfWork.CommitAsync();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);
            _acessosUnitOfWork.Untrack<TokenRedefinicaoSenha>(tokenRedefinicaoSenha);

            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = tokenRedefinicaoSenha.Token,
                NovaSenha = novaSenha
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var usuarioAutenticado = await _usuarioRepository.PesquisarPorEmailESenhaAsync(email, novaSenha);
            Assert.NotNull(usuarioAutenticado);
            Assert.Equal(email, usuarioAutenticado.Email);

            var usuarioComSenhaAntiga = await _usuarioRepository.PesquisarPorEmailESenhaAsync(email, senhaOriginal);
            Assert.Null(usuarioComSenhaAntiga);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_redefinir_senha_com_token_invalido()
        {
            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = "token_inexistente",
                NovaSenha = "NovaSenha@123"
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);

            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_redefinir_senha_com_token_expirado()
        {
            var usuario = await CadastrarUsuario();
            
            // Cria token com data de expiração no passado
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id, horasExpiracao: -1);
            await _tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
            await _acessosUnitOfWork.CommitAsync();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);
            _acessosUnitOfWork.Untrack<TokenRedefinicaoSenha>(tokenRedefinicaoSenha);

            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = tokenRedefinicaoSenha.Token,
                NovaSenha = "NovaSenha@123"
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);

            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_redefinir_senha_com_token_ja_utilizado()
        {
            var usuario = await CadastrarUsuario();
            
            // Cria token e marca como utilizado
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id);
            tokenRedefinicaoSenha.MarcarComoUtilizado();
            await _tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
            await _acessosUnitOfWork.CommitAsync();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);
            _acessosUnitOfWork.Untrack<TokenRedefinicaoSenha>(tokenRedefinicaoSenha);

            var redefinirSenhaViewModel = new Application.ViewModels.UsuarioRedefinirSenhaViewModel
            {
                Token = tokenRedefinicaoSenha.Token,
                NovaSenha = "NovaSenha@123"
            };

            await _usuarioService.RedefinirSenhaAsync(redefinirSenhaViewModel);

            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_tokens_antigos_ao_solicitar_nova_redefinicao()
        {
            var email = "usuario@test.com";
            var usuario = await CadastrarUsuario(email: email);
            
            // Cria primeiro token
            var primeiroToken = new TokenRedefinicaoSenha(usuario.Id);
            await _tokenRedefinicaoSenhaRepository.Cadastrar(primeiroToken);
            await _acessosUnitOfWork.CommitAsync();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);
            _acessosUnitOfWork.Untrack<TokenRedefinicaoSenha>(primeiroToken);

            // Solicita nova redefinição
            var solicitarRedefinicaoViewModel = new Application.ViewModels.SolicitarRedefinicaoSenhaViewModel
            {
                Email = email
            };

            await _usuarioService.SolicitarRedefinicaoSenhaAsync(solicitarRedefinicaoViewModel);

            // Verifica se o primeiro token foi invalidado
            var primeiroTokenAtualizado = await _tokenRedefinicaoSenhaRepository.PesquisarPorTokenAsync(primeiroToken.Token);
            Assert.True(primeiroTokenAtualizado.Utilizado);
        }

        [Fact]
        public async Task Deve_enviar_email_confirmacao_apos_cadastro()
        {
            var viewModel = RegistrarUsuarioViewModelMoq.GetUsuarioViewModel();

            var usuarioId = await _usuarioService.CadastrarAsync(viewModel);

            Assert.NotEqual(Guid.Empty, usuarioId);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_enviar_email_confirmacao_para_usuario_existente()
        {
            var usuario = await CadastrarUsuario();
            _acessosUnitOfWork.Untrack<Usuario>(usuario);

            var resultado = await _usuarioService.EnviarEmailConfirmacaoAsync(usuario.Id);

            Assert.True(resultado);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_enviar_email_confirmacao_usuario_inexistente()
        {
            var usuarioId = Guid.NewGuid();

            var resultado = await _usuarioService.EnviarEmailConfirmacaoAsync(usuarioId);

            Assert.False(resultado);
            Assert.True(_notificationContext.HasErrors);
        }

        private async Task<Usuario> CadastrarUsuario(
            Guid? id = null,
            string nome = null,
            string email = null,
            string senha = null,
            EStatusUsuario status = EStatusUsuario.PendenteConfirmacao)
        {
            var usuario = UsuarioMoq.GetUsuario(
                id: id,
                nome: nome ?? "Usuario Teste",
                email: email ?? $"{Guid.NewGuid()}@test.com",
                senha: senha,
                status: status
            );
            await _usuarioRepository.Cadastrar(usuario);
            await _acessosUnitOfWork.CommitAsync();
            return usuario;
        }
    }
}
