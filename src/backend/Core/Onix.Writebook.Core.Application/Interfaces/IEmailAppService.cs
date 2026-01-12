using Onix.Writebook.Core.Application.ViewModels;
using System.Threading.Tasks;

namespace Onix.Writebook.Core.Application.Interfaces
{
    public interface IEmailAppService
    {
        Task<bool> EnviarEmailAsync(EmailViewModel emailViewModel);
        Task<bool> EnviarEmailConfirmacaoAsync(EmailConfirmacaoViewModel emailConfirmacaoViewModel);
        Task<bool> EnviarEmailRedefinicaoSenhaAsync(EmailRedefinicaoSenhaViewModel emailRedefinicaoSenhaViewModel);
    }
}
