using Onix.Framework.Infra.Data.EFCore;
using Onix.Writebook.Books.Domain.Entities;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Books.Infra.Data.Context;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Infra.Data.Repositories
{
    public class UsuarioRepository(BooksDbContext context) : EFCoreRepository<Usuario>(context), IUsuarioRepository
    {
        public async Task<bool> Exists(Guid id)
        {
            return await base.AnyAsync<Usuario>(x => x.Id == id);
        }
    }
}
