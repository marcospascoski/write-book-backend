using System;

namespace Onix.Writebook.Acesso.Application.ViewModels
{
    public class UsuarioViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public int Status { get; set; }
        public string AccessToken { get; internal set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CriarUsuarioViewModel
    {
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
