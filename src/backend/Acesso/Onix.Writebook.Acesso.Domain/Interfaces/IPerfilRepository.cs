using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IPerfilRepository : IRepository<Perfil>
    {
        Task<int> CountAsync();
        Task Cadastrar(Perfil perfil);
        void Alterar(Perfil perfil);
        Task<Perfil> PesquisarPorIdAsync(long id);
        Task<IPagedItems<Perfil>> Paginar(IPaged paged, string texto);
        Task<bool> JaCadastrado(string nome, long id);
    }
}
