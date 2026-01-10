using Onix.Framework.Infra.Data.EFCore;
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
    public class PerfilPermissaoRepository : EFCoreRepository<PerfilPermissao>, IPerfilPermissaoRepository
    {
        public PerfilPermissaoRepository(AcessosDbContext context) : base(context)
        {
        }

        public Task<bool> AnyAsync(long id)
        {
            return base.AnyAsync<PerfilPermissao>();
        }
        public async Task Cadastrar(PerfilPermissao perfilPermissao)
        {
            await base.AddAsync<PerfilPermissao>(perfilPermissao);
        }
        public void Alterar(PerfilPermissao perfilPermissao)
        {
            base.Update<PerfilPermissao>(perfilPermissao);
        }
        public void Remover(PerfilPermissao perfilPermissao)
        {
            base.Remove<PerfilPermissao>(perfilPermissao);
        }
        public async Task<PerfilPermissao> PesquisarPorIdAsync(long id)
        {
            return await base.FindByIdAsync<PerfilPermissao, long>(id);
        }
        public async Task<bool> JaCadastrado(long perfilId, long permissaoId)
        {
            return await base.AnyAsync<PerfilPermissao>(x => x.PerfilId.Equals(perfilId) && x.PermissaoId == permissaoId);
        }
    }
}
