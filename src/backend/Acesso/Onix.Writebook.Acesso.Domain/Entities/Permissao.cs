using Onix.Writebook.Core.Domain.Entities;
using Onix.Writebook.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class Permissao : AggregateRootEntity
    {
        public long Id { get; private set; }
        public string Nome { get; private set; }
        public EStatusEntidade Status { get; private set; }
        protected Permissao() { }

        public Permissao(
            long id,
            string nome
        )
        {
            Id = id;
            Nome = nome;
            Status = EStatusEntidade.Inativo;
        }

        public void AlterarDados(Permissao permissaoAlterarDados)
        {
            Nome = permissaoAlterarDados.Nome;
        }

        public void AlterarStatus(EStatusEntidade status)
        {
            this.Status = status;
        }
        public static class Factory
        {
            public static Permissao Create(Permissao prototype)
            {
                return new Permissao
                {
                    Nome = prototype.Nome,
                    Status = EStatusEntidade.Inativo
                };
            }
        }
    }
}
