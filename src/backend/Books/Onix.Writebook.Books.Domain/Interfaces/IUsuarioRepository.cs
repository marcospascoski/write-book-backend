using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Books.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Domain.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<bool> Exists(Guid id);
    }
}
