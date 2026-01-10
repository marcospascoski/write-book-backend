using System;
using System.Threading.Tasks;
using Xunit;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Tests.Moqs;
using Onix.Framework.Notifications.Interfaces;
using System.Linq;

namespace Onix.Writebook.Acesso.Tests.Validators
{
    public class UsuarioValidatorTests(
        IUsuarioValidator usuarioValidator,
        INotificationContext notificationContext)
    {
        private readonly IUsuarioValidator _usuarioValidator = usuarioValidator;
        private readonly INotificationContext _notificationContext = notificationContext;


        [Fact]
        public async Task Deve_validar_usuario_valida()
        {
            var usuario = UsuarioMoq.GetUsuario();

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.True(result);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_nome_vazio()
        {
            var usuario = UsuarioMoq.GetUsuario(nome: "");

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_nome_null()
        {
            var usuario = UsuarioMoq.GetUsuario(nome: null);

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_email_vazio()
        {
            var usuario = UsuarioMoq.GetUsuario(email: "");

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_email_invalido()
        {
            var usuario = UsuarioMoq.GetUsuario(email: "emailinvalido");

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_email_null()
        {
            var usuario = UsuarioMoq.GetUsuario(email: null);

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_invalidar_usuario_com_id_vazio()
        {
            var usuario = UsuarioMoq.GetUsuario(id: Guid.Empty);

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.False(result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_validar_usuario_com_email_valido_multiplos_formatos()
        {
            var emailsValidos = new[] 
            { 
                "usuario@test.com",
                "usuario.sobrenome@test.com.br",
                "usuario+tag@test.co.uk"
            };

            foreach (var email in emailsValidos)
            {
                _notificationContext.Clear();
                var usuario = UsuarioMoq.GetUsuario(email: email);

                var result = await _usuarioValidator.IsValid(usuario);

                Assert.True(result, $"Email {email} deveria ser válido");
            }
        }

        [Fact]
        public async Task Deve_validar_usuario_ativo()
        {
            var usuario = UsuarioMoq.GetUsuarioAtivo();

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.True(result);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_validar_usuario_bloqueado()
        {
            var usuario = UsuarioMoq.GetUsuarioBloqueado();

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.True(result);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_validar_usuario_inativo()
        {
            var usuario = UsuarioMoq.GetUsuarioInativo();

            var result = await _usuarioValidator.IsValid(usuario);

            Assert.True(result);
            Assert.False(_notificationContext.HasErrors);
        }
    }
}
