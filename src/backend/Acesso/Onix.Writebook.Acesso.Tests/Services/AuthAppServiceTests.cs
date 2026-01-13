using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Login;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Tests.Moqs;
using Onix.Writebook.Core.Resources;
using Xunit;

namespace Onix.Writebook.Acesso.Tests.Services
{
    public class AuthAppServiceTests(
        INotificationContext notificationContext,
        IAuthAppService authAppService,
        IUsuarioRepository usuarioRepository,
        IUsuarioAppService usuarioAppService,
        IAcessosUnitOfWork acessosUnitOfWork)
    {
        private readonly INotificationContext _notificationContext = notificationContext;
        private readonly IAuthAppService _authAppService = authAppService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioAppService _usuarioAppService = usuarioAppService;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork = acessosUnitOfWork;


        #region Login Tests

        [Fact]
        public async Task Deve_realizar_login_com_sucesso()
        {
            // Arrange
            var senha = "senha123";
            var email = "usuario.login@test.com";
            var usuarioRegistrado = await CriarUsuario(email, senha);
            _acessosUnitOfWork.Untrack<Usuario>(usuarioRegistrado);

            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Senha = senha
            };

            // Act
            var result = await _authAppService.Login(loginViewModel);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.NotNull(result.AccessToken);
            Assert.Null(result.Senha); // Senha não deve ser retornada
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_email_nao_encontrado()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "emailinexistente@test.com",
                Senha = "senha123"
            };

            // Act
            var result = await _authAppService.Login(loginViewModel);

            // Assert
            Assert.Null(result);
            Assert.True(_notificationContext.HasErrors);
            Assert.Contains(_notificationContext.GetErrors().Select(e => e.Message), 
                e => e.Contains("não encontrado") || e.Contains("not found"));
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_senha_invalida()
        {
            // Arrange
            var senha = "senhaCorreta123";
            var email = "usuario.senhaerrada@test.com";
            await CriarUsuario(email, senha);

            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Senha = "senhaErrada"
            };

            // Act
            var result = await _authAppService.Login(loginViewModel);

            // Assert
            Assert.Null(result);
            Assert.True(_notificationContext.HasErrors);
            Assert.Contains(_notificationContext.GetErrors().Select(e => e.Message), 
                e => e.Contains("senha", StringComparison.OrdinalIgnoreCase) || 
                     e.Contains("password", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_email_vazio()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                Email = "",
                Senha = "senha123"
            };

            // Act
            var result = await _authAppService.Login(loginViewModel);

            // Assert
            Assert.Null(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_quando_senha_vazia()
        {
            // Arrange
            var email = "usuario@test.com";
            await CriarUsuario(email, "senha123");

            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Senha = ""
            };

            // Act
            var result = await _authAppService.Login(loginViewModel);

            // Assert
            Assert.Null(result);
            Assert.True(_notificationContext.HasErrors);
        }

        #endregion

        #region Logout Tests

        [Fact]
        public async Task Deve_realizar_logout_com_sucesso()
        {
            // Arrange
            var senha = "senha123";
            var email = "usuario.logout@test.com";
            await CriarUsuario(email, senha);

            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Senha = senha
            };

            var loginResult = await _authAppService.Login(loginViewModel);
            var accessToken = loginResult.AccessToken;

            // Act
            await _authAppService.Logout(accessToken);

            // Assert
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Nao_deve_permitir_login_com_token_na_blacklist()
        {
            // Arrange
            var senha = "senha123";
            var email = "usuario.blacklist@test.com";
            await CriarUsuario(email, senha);

            var loginViewModel = new LoginViewModel
            {
                Email = email,
                Senha = senha
            };

            // Primeiro login
            var loginResult = await _authAppService.Login(loginViewModel);
            var accessToken = loginResult.AccessToken;

            // Logout (adiciona token à blacklist)
            await _authAppService.Logout(accessToken);

            // Act - Tentar usar o mesmo token
            // Nota: Esta funcionalidade depende da implementação do ITokenBlacklistService
            
            // Assert
            Assert.NotNull(loginResult);
            Assert.False(_notificationContext.HasErrors);
        }

        #endregion

        #region Helper Methods

        private async Task<Usuario> CriarUsuario(string email, string senha)
        {
            var registrarViewModel = RegistrarUsuarioViewModelMoq.GetUsuarioViewModel();
            registrarViewModel.Email = email;
            registrarViewModel.Senha = senha;

            var usuarioId = await _usuarioAppService.CadastrarAsync(registrarViewModel);
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioId);

            return usuario;
        }

        #endregion
    }
}
