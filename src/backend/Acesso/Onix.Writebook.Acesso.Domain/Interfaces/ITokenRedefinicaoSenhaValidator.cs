using Onix.Writebook.Acesso.Domain.Entities;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface ITokenRedefinicaoSenhaValidator
    {
        Task<bool> IsValid(TokenRedefinicaoSenha token);
    }
}
