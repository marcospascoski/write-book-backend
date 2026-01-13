using Onix.Framework.Security;
using Onix.Writebook.Core.Domain.Entities;
using System;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class RefreshToken : AggregateRootEntity
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public virtual Usuario Usuario { get; private set; }
        public string Token { get; private set; }
        public DateTime DataExpiracao { get; private set; }
        public bool Revogado { get; private set; }
        public DateTime? DataRevogacao { get; private set; }
        public string IPAddress { get; private set; }
        public string UserAgent { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected RefreshToken() { }

        public RefreshToken(
            Guid usuarioId,
            string ipAddress,
            string userAgent,
            int diasExpiracao = 30)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Token = GenerateSecureToken();
            DataExpiracao = DateTime.UtcNow.AddDays(diasExpiracao);
            Revogado = false;
            IPAddress = ipAddress;
            UserAgent = userAgent;
            CreatedAt = DateTime.UtcNow;
        }

        private static string GenerateSecureToken()
        {
            return EncryptionHelper.GenerateRandomSecret(32);
        }

        public bool EstaValido()
        {
            return !Revogado && DateTime.UtcNow <= DataExpiracao;
        }

        public void Revogar()
        {
            Revogado = true;
            DataRevogacao = DateTime.UtcNow;
        }

        public bool EstaExpirado()
        {
            return DateTime.UtcNow > DataExpiracao;
        }
        public bool CompararToken(string tokenParaComparar)
        {
            if (string.IsNullOrEmpty(tokenParaComparar))
                return false;

            // Usa SlowEquals do EncryptionHelper para comparação segura
            return EncryptionHelper.SlowEquals(Token, tokenParaComparar);
        }

                public static class Factory
                {
                    public static RefreshToken Create(
                        Guid usuarioId,
                        string ipAddress,
                        string userAgent,
                        int diasExpiracao = 30)
                    {
                        return new RefreshToken(
                            usuarioId,
                            ipAddress,
                            userAgent,
                            diasExpiracao
                        );
                    }
                }
            }
        }