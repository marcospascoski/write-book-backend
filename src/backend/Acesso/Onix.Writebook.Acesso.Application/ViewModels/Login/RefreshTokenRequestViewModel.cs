using System.ComponentModel.DataAnnotations;

namespace Onix.Writebook.Acesso.Application.ViewModels.Login
{
    public class RefreshTokenRequestViewModel
    {
        [Required(ErrorMessage = "O Refresh Token é obrigatório")]
        public string RefreshToken { get; set; }
    }
}
