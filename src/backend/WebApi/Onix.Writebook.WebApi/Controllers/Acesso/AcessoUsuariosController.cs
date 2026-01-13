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
    [Route("api/usuario")]
    public class AcessoUsuariosController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IUsuarioAppService usuarioAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        [HttpGet, Route("perfil")]
        public async Task<IActionResult> PesquisarPerfil(Guid userId)
        {
            return await TryExecuteAsync(usuarioAppService.PesquisarPorId(userId));
        }

        [HttpPut, Route("perfil")]
        public async Task<IActionResult> AlterarPerfil(Guid userId, UsuarioViewModel model)
        {
            return await TryExecuteAsync(usuarioAppService.AlterarPerfilAsync(userId, model));
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarAsync(RegistrarUsuarioViewModel model)
        {
            return await TryExecuteAsync(usuarioAppService.CadastrarAsync(model));
        }

        [HttpPut]
        public async Task<IActionResult> Alterar(UsuarioViewModel model)
        {
            return await TryExecuteNoResultAsync(usuarioAppService.Alterar(model));
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> PesquisarPorId(Guid id)
        {
            return await TryExecuteAsync(usuarioAppService.PesquisarPorId(id));
        }

        [HttpPatch, Route("status")]
        public async Task<IActionResult> AlterarStatus(UsuarioAlterarStatusViewModel model)
        {
            return await TryExecuteNoResultAsync(usuarioAppService.AlterarStatus(model));
        }

        [HttpPost, Route("paginar")]
        public async Task<IActionResult> Paginar(FiltroUsuarioViewModel model)
        {
            return await TryExecuteAsync(usuarioAppService.Paginar(model));
        }

        [HttpPost, Route("solicitar-redefinicao-senha")]
        public async Task<IActionResult> SolicitarRedefinicaoSenha(SolicitarRedefinicaoSenhaViewModel model)
        {
            return await TryExecuteAsync(usuarioAppService.SolicitarRedefinicaoSenhaAsync(model));
        }

        [HttpPatch, Route("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha(UsuarioRedefinirSenhaViewModel model)
        {
            return await TryExecuteNoResultAsync(usuarioAppService.RedefinirSenhaAsync(model));
        }
    }
}
