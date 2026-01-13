using Onix.Framework.Infra.Data.EFCore;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Infra.Data.Repositories
{
    public class PermissaoRepository(AcessosDbContext context) : EFCoreRepository<Permissao>(context), IPermissaoRepository
    {
        public async Task<int> CountAsync()
        {
            return await base.CountAsync<Permissao>();
        }

        public async Task Cadastrar(Permissao perfil)
        {
            await base.AddAsync<Permissao>(perfil);
        }
        public void Alterar(Permissao perfil)
        {
            base.Update<Permissao>(perfil);
        }
        public async Task<Permissao> PesquisarPorIdAsync(long id)
        {
            return await base.FindByIdAsync<Permissao, long>(id);
        }

        public async Task<IPagedItems<Permissao>> Paginar(IPaged paged, string texto)
        {
            var query = base.Queryable<Permissao>();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(x => x.Nome.Contains(texto));
            }
            return await base.GetPagedAsync<Permissao>(paged, query);
        }

        public async Task<bool> JaCadastrado(string nome, long id)
        {
            return await base.AnyAsync<Permissao>(x => x.Nome.Equals(nome) && x.Id != id);
        }
    }
}
