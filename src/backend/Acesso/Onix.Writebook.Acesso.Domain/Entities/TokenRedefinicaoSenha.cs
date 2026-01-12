using Onix.Writebook.Core.Domain.Entities;
using System;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class TokenRedefinicaoSenha : AggregateRootEntity
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public virtual Usuario Usuario { get; private set; }
        public string Token { get; private set; }
        public DateTime DataExpiracao { get; private set; }
        public bool Utilizado { get; private set; }
        public DateTime? DataUtilizacao { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected TokenRedefinicaoSenha() { }

        public TokenRedefinicaoSenha(Guid usuarioId, int horasExpiracao = 24)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            Token = Guid.NewGuid().ToString("N");
            DataExpiracao = DateTime.UtcNow.AddHours(horasExpiracao);
            Utilizado = false;
            CreatedAt = DateTime.UtcNow;
        }

        public bool EstaValido()
        {
            return !Utilizado && DateTime.UtcNow <= DataExpiracao;
        }

        public void MarcarComoUtilizado()
        {
            Utilizado = true;
            DataUtilizacao = DateTime.UtcNow;
        }

        public bool EstaExpirado()
        {
            return DateTime.UtcNow > DataExpiracao;
        }
    }
}
