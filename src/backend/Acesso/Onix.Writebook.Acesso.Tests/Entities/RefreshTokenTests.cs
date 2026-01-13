using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Tests.Moqs;
using System;
using Xunit;

namespace Onix.Writebook.Acesso.Tests.Entities
{
    public class RefreshTokenTests
    {
        #region Constructor Tests

        [Fact]
        public void Deve_criar_refresh_token_com_sucesso()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var ipAddress = "192.168.1.100";
            var userAgent = "Mozilla/5.0";

            // Act
            var refreshToken = new RefreshToken(usuarioId, ipAddress, userAgent);

            // Assert
            Assert.NotEqual(Guid.Empty, refreshToken.Id);
            Assert.Equal(usuarioId, refreshToken.UsuarioId);
            Assert.Equal(ipAddress, refreshToken.IPAddress);
            Assert.Equal(userAgent, refreshToken.UserAgent);
            Assert.NotNull(refreshToken.Token);
            Assert.NotEmpty(refreshToken.Token);
            Assert.False(refreshToken.Revogado);
            Assert.Null(refreshToken.DataRevogacao);
            Assert.True(refreshToken.DataExpiracao > DateTime.UtcNow);
        }

        [Fact]
        public void Deve_criar_token_com_expiracao_personalizada()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var diasExpiracao = 60;

            // Act
            var refreshToken = new RefreshToken(usuarioId, "192.168.1.1", "Test", diasExpiracao);

            // Assert
            var diasCalculados = (refreshToken.DataExpiracao - DateTime.UtcNow).TotalDays;
            Assert.True(diasCalculados >= 59 && diasCalculados <= 60);
        }

        [Fact]
        public void Deve_gerar_tokens_unicos_para_cada_criacao()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act
            var token1 = new RefreshToken(usuarioId, "192.168.1.1", "Test");
            var token2 = new RefreshToken(usuarioId, "192.168.1.1", "Test");

            // Assert
            Assert.NotEqual(token1.Token, token2.Token);
            Assert.NotEqual(token1.Id, token2.Id);
        }

        #endregion

        #region Factory.Create Tests

        [Fact]
        public void Factory_Create_deve_criar_token_com_sucesso()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var ipAddress = "10.0.0.1";
            var userAgent = "Chrome/120.0";

            // Act
            var refreshToken = RefreshToken.Factory.Create(usuarioId, ipAddress, userAgent);

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(usuarioId, refreshToken.UsuarioId);
            Assert.Equal(ipAddress, refreshToken.IPAddress);
            Assert.Equal(userAgent, refreshToken.UserAgent);
            Assert.False(refreshToken.Revogado);
            Assert.True(refreshToken.EstaValido());
        }

        [Fact]
        public void Factory_Create_deve_gerar_novo_token_e_id()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();

            // Act
            var token1 = RefreshToken.Factory.Create(usuarioId, "192.168.1.1", "Test");
            var token2 = RefreshToken.Factory.Create(usuarioId, "192.168.1.1", "Test");

            // Assert
            Assert.NotEqual(token1.Id, token2.Id);
            Assert.NotEqual(token1.Token, token2.Token);
        }

        [Fact]
        public void Factory_Create_deve_aceitar_dias_expiracao_customizados()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var diasExpiracao = 60;

            // Act
            var token = RefreshToken.Factory.Create(usuarioId, "192.168.1.1", "Test", diasExpiracao);

            // Assert
            var diasCalculados = (token.DataExpiracao - DateTime.UtcNow).TotalDays;
            Assert.True(diasCalculados >= 59 && diasCalculados <= 60);
        }

        #endregion

        #region Legacy Factory Tests (if exists)

        #endregion

        #region EstaValido Tests

        [Fact]
        public void EstaValido_deve_retornar_true_para_token_ativo()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();

            // Act
            var resultado = refreshToken.EstaValido();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void EstaValido_deve_retornar_false_para_token_revogado()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenRevogado();

            // Act
            var resultado = refreshToken.EstaValido();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void EstaValido_deve_retornar_false_para_token_expirado()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenExpirado();

            // Act
            var resultado = refreshToken.EstaValido();

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Revogar Tests

        [Fact]
        public void Revogar_deve_marcar_token_como_revogado()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();
            Assert.False(refreshToken.Revogado);
            Assert.Null(refreshToken.DataRevogacao);

            // Act
            refreshToken.Revogar();

            // Assert
            Assert.True(refreshToken.Revogado);
            Assert.NotNull(refreshToken.DataRevogacao);
            Assert.True(refreshToken.DataRevogacao <= DateTime.UtcNow);
        }

        [Fact]
        public void Token_revogado_nao_deve_estar_valido()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();

            // Act
            refreshToken.Revogar();

            // Assert
            Assert.False(refreshToken.EstaValido());
        }

        #endregion

        #region EstaExpirado Tests

        [Fact]
        public void EstaExpirado_deve_retornar_false_para_token_valido()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();

            // Act
            var resultado = refreshToken.EstaExpirado();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void EstaExpirado_deve_retornar_true_para_token_expirado()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenExpirado();

            // Act
            var resultado = refreshToken.EstaExpirado();

            // Assert
            Assert.True(resultado);
        }

        #endregion

        #region CompararToken Tests

        [Fact]
        public void CompararToken_deve_retornar_true_para_token_identico()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();
            var tokenString = refreshToken.Token;

            // Act
            var resultado = refreshToken.CompararToken(tokenString);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void CompararToken_deve_retornar_false_para_token_diferente()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();
            var outroToken = "token_diferente_123456789";

            // Act
            var resultado = refreshToken.CompararToken(outroToken);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void CompararToken_deve_retornar_false_para_token_vazio()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();

            // Act
            var resultado = refreshToken.CompararToken("");

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void CompararToken_deve_retornar_false_para_token_null()
        {
            // Arrange
            var refreshToken = RefreshTokenMoq.GetRefreshTokenValido();

            // Act
            var resultado = refreshToken.CompararToken(null);

            // Assert
            Assert.False(resultado);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Ciclo_completo_token_deve_funcionar_corretamente()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var ipAddress = "192.168.1.50";
            var userAgent = "Firefox/120.0";

            // Act & Assert
            // 1. Criar token
            var token = RefreshToken.Factory.Create(usuarioId, ipAddress, userAgent);
            Assert.True(token.EstaValido());
            Assert.False(token.EstaExpirado());

            // 2. Comparar token
            var tokenString = token.Token;
            Assert.True(token.CompararToken(tokenString));

                        // 3. Revogar token
                        token.Revogar();
                        Assert.False(token.EstaValido());
                        Assert.True(token.Revogado);

                        // 4. Criar novo token com mesmos dados
                        var novoToken = RefreshToken.Factory.Create(token.UsuarioId, token.IPAddress, token.UserAgent);
                        Assert.True(novoToken.EstaValido());
                        Assert.False(novoToken.Revogado);
                        Assert.NotEqual(token.Token, novoToken.Token);
                    }

                    #endregion
                }
            }
