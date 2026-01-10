using Onix.Writebook.Acesso.Domain.Entities;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IUsuarioValidator
    {
        Task<bool> IsValid(Usuario usuario);
    }
}
