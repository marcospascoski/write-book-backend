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
            Guid id,
            Guid usuarioId,
            string ipAddress,
            string userAgent,
            int diasExpiracao = 30)
        {
            Id = id;
            UsuarioId = usuarioId;
            Token = Guid.NewGuid().ToString("N");
            DataExpiracao = DateTime.UtcNow.AddDays(diasExpiracao);
            Revogado = false;
            IPAddress = ipAddress;
            UserAgent = userAgent;
            CreatedAt = DateTime.UtcNow;
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

        public static class Factory
        {
            public static RefreshToken Create(RefreshToken prototype)
            {
                return new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = prototype.UsuarioId,
                    IPAddress = prototype.IPAddress,
                    UserAgent = prototype.UserAgent,
                    DataExpiracao = prototype.DataExpiracao
                };
            }
        }
    }
}