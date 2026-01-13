using System;

namespace Onix.Writebook.Acesso.Application.ViewModels
{
    public class UsuarioViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Status { get; set; }
        public string AccessToken { get; internal set; }
        public string RefreshToken { get; internal set; }
        public DateTime CreatedAt { get; set; }
    }

}
