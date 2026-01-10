using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/acesso/usuario")]
    public class AcessoUsuariosController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IUsuarioAppService usuarioAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        private readonly IUsuarioAppService _usuarioAppService = usuarioAppService;

        [HttpGet, Route("perfil")]
        public async Task<IActionResult> PesquisarPerfil([FromQuery] Guid userId)
        {
            return await TryExecuteAsync(_usuarioAppService.PesquisarPorId(userId));
        }

        [HttpPut, Route("perfil")]
        public async Task<IActionResult> AlterarPerfil([FromQuery] Guid userId, [FromBody] UsuarioViewModel model)
        {
            return await TryExecuteAsync(_usuarioAppService.AlterarPerfilAsync(userId, model));
        }

        [HttpPost, Route("usuarios")]
        public async Task<IActionResult> CadastrarAsync(RegistrarUsuarioViewModel model)
        {
            return await TryExecuteAsync(_usuarioAppService.CadastrarAsync(model));
        }

        [HttpPut, Route("usuarios")]
        public async Task<IActionResult> Alterar(UsuarioViewModel model)
        {
            return await TryExecuteNoResultAsync(_usuarioAppService.Alterar(model));
        }

        [HttpGet, Route("usuarios/{id}")]
        public async Task<IActionResult> PesquisarPorId(Guid id)
        {
            return await TryExecuteAsync(_usuarioAppService.PesquisarPorId(id));
        }

        [HttpPatch, Route("usuarios/status")]
        public async Task<IActionResult> AlterarStatus(UsuarioAlterarStatusViewModel model)
        {
            return await TryExecuteNoResultAsync(_usuarioAppService.AlterarStatus(model));
        }

        [HttpPost, Route("usuarios/paginar")]
        public async Task<IActionResult> Paginar(FiltroUsuarioViewModel model)
        {
            return await TryExecuteAsync(_usuarioAppService.Paginar(model));
        }        
    }
}
