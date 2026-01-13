using System;

namespace Onix.Writebook.Acesso.Application.ViewModels
{
    public class RefreshTokenViewModel
    {
        public Guid UsuarioId { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
