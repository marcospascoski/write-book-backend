using Onix.Writebook.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class PerfilPermissao : AggregateRootEntity
    {
        public long Id { get; private set; }
        public long PerfilId { get; private set; }
        public virtual Perfil Perfil { get; private set; }
        public long PermissaoId { get; private set; }
        public virtual Permissao Permissao { get; private set; }

        protected PerfilPermissao() { }

        public PerfilPermissao(
            long id,
            long perfilId,
            long permissaoId
        )
        {
            Id = id;
            PerfilId = perfilId;
            PermissaoId = permissaoId;
        }

        public static class Factory
        {
            public static PerfilPermissao Create(PerfilPermissao prototype)
            {
                return new PerfilPermissao
                {
                    PerfilId = prototype.PerfilId,
                    PermissaoId = prototype.PermissaoId
                };
            }
        }
    }
}
