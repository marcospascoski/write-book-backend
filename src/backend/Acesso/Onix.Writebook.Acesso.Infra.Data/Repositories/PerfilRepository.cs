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
    public class PerfilRepository(AcessosDbContext context) : EFCoreRepository<Perfil>(context), IPerfilRepository
    {
        public async Task<int> CountAsync()
        {
            return await base.CountAsync<Perfil>();
        }

        public async Task Cadastrar(Perfil perfil)
        {
            await base.AddAsync<Perfil>(perfil);
        }
        public void Alterar(Perfil perfil)
        {
            base.Update<Perfil>(perfil);
        }
        public async Task<Perfil> PesquisarPorIdAsync(long id)
        {
            return await base.FindByIdAsync<Perfil, long>(id);
        }

        public async Task<IPagedItems<Perfil>> Paginar(IPaged paged, string texto)
        {
            var query = base.Queryable<Perfil>();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(x => x.Nome.Contains(texto));
            }
            return await base.GetPagedAsync<Perfil>(paged, query);
        }

        public async Task<bool> JaCadastrado(string nome, long id)
        {
            return await base.AnyAsync<Perfil>(x => x.Nome.Equals(nome) && x.Id != id);
        }
    }
}
