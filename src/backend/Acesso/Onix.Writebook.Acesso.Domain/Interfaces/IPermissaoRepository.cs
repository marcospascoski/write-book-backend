using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IPermissaoRepository : IRepository<Permissao>
    {
        Task<int> CountAsync();
        Task Cadastrar(Permissao perfil);
        void Alterar(Permissao perfil);
        Task<Permissao> PesquisarPorIdAsync(long id);
        Task<IPagedItems<Permissao>> Paginar(IPaged paged, string texto);
        Task<bool> JaCadastrado(string nome, long id);
    }
}
