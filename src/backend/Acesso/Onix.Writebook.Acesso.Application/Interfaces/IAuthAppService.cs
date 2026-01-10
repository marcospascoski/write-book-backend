using System.Threading.Tasks;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Login;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IAuthAppService
    {
        Task<UsuarioViewModel> Login(LoginViewModel loginViewModel);
        Task Logout(string accessToken);
    }

    

    

    
}
