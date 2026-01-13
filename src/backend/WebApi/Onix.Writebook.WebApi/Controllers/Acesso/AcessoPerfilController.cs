using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/perfis")]
    public class AcessoPerfilController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IPerfilAppService perfilAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Cadastrar(PerfilViewModel model)
        {
            return await TryExecuteAsync(perfilAppService.Cadastrar(model));
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Alterar(PerfilViewModel model)
        {
            return await TryExecuteNoResultAsync(perfilAppService.Alterar(model));
        }

        [HttpGet, Route("{id}")]
        [Authorize(Policy = "ClienteOnly")]
        public async Task<IActionResult> PesquisarPorId(long id)
        {
            return await TryExecuteAsync(perfilAppService.PesquisarPorId(id));
        }

        [HttpPatch, Route("status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AlterarStatus(PerfilAlterarStatusViewModel<long> model)
        {
            return await TryExecuteNoResultAsync(perfilAppService.AlterarStatus(model));
        }

        [HttpPost, Route("paginar")]
        [Authorize(Policy = "ClienteOnly")]
        public async Task<IActionResult> Paginar(FiltroPerfilViewModel model)
        {
            return await TryExecuteAsync(perfilAppService.Paginar(model));
        }
    }
}