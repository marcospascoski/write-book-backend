using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IPerfilPermissaoRepository : IRepository<PerfilPermissao>
    {
        Task<bool> AnyAsync(long id);
        Task Cadastrar(PerfilPermissao perfilPermissao);
        void Remover(PerfilPermissao perfilPermissao);
        Task<PerfilPermissao> PesquisarPorIdAsync(long id);
        Task<bool> JaCadastrado(long perfilId, long permissaoId);
    }
}
