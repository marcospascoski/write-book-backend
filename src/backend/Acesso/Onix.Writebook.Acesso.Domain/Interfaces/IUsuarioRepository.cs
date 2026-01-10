using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<int> CountAsync();
        Task Cadastrar(Usuario usuario);
        void Alterar(Usuario usuario);
        Task<Usuario> PesquisarPorIdAsync(Guid id);
        Task<Usuario> PesquisarPorEmailAsync(string email);
        Task<Usuario> PesquisarPorEmailESenhaAsync(string email, string senha);
        Task<IPagedItems<Usuario>> Paginar(IPaged paged, string texto);
        Task<bool> JaCadastrado(string email, Guid id);
        Task<bool> Exists(Guid id);
    }
}
