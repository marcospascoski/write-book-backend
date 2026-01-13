using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Infra.Data.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Infra.Data.Repositories
{
    public class UsuarioRepository(AcessosDbContext context) 
        : EFCoreRepository<Usuario>(context), IUsuarioRepository
    {
        public async Task<int> CountAsync()
        {
            return await base.CountAsync<Usuario>();
        }

        public async Task Cadastrar(Usuario usuario)
        {
            await base.AddAsync<Usuario>(usuario);
        }
        public void Alterar(Usuario usuario)
        {
            base.Update<Usuario>(usuario);
        }
        public async Task<Usuario> PesquisarPorIdAsync(Guid id)
        {
            return await base.FindByIdAsync<Usuario, Guid>(id);
        }

        public async Task<Usuario> PesquisarPorEmailAsync(string email)
        {
            return await base.FirstOrDefaultAsync<Usuario>(x => x.Email.Equals(email));
        }

        public async Task<IPagedItems<Usuario>> Paginar(IPaged paged, string texto)
        {
            var query = base.Queryable<Usuario>();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(x => x.Email.Contains(texto) || x.Nome.Contains(texto));
            }
            return await base.GetPagedAsync<Usuario>(paged, query);
        }

        public async Task<bool> JaCadastrado(string email, Guid id)
        {
            return await base.AnyAsync<Usuario>(x => x.Email.Equals(email) && x.Id != id);
        }

        public async Task<bool> Exists(Guid id)
        {
            return await base.AnyAsync<Usuario>(x => x.Id == id);
        }

        public async Task<Usuario> PesquisarPorEmailESenhaAsync(string email, string senha)
        {
            // Compare using hashed password with salt
            var usuario = await base.FirstOrDefaultAsync<Usuario>(x => x.Email == email);
            if (usuario == null) return null;
            return usuario.Senha.ValidarSenha(senha, usuario.Salt.Valor) ? usuario : null;
        }
    }
}
