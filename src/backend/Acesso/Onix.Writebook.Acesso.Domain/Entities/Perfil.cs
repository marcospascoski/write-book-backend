using Onix.Writebook.Core.Domain.Entities;
using Onix.Writebook.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class Perfil : AggregateRootEntity
    {
        public long Id { get; private set; }
        public string Nome { get; private set; }
        public virtual ICollection<PerfilPermissao> PerfilPermissoes { get; private set; }
        public EStatusEntidade Status { get; private set; }

        protected Perfil() { }

        public Perfil(
            long id,
            string nome
        )
        {
            Id = id;
            Nome = nome;
        }

        public void AlterarDados(Perfil perfilAlterarDados)
        {
            Nome = perfilAlterarDados.Nome;
        }

        public void AlterarStatus(EStatusEntidade status)
        {
            this.Status = status;
        }
        public static class Factory
        {
            public static Perfil Create(Perfil prototype)
            {
                return new Perfil
                {
                    Nome = prototype.Nome,
                };
            }
        }
    }
}
