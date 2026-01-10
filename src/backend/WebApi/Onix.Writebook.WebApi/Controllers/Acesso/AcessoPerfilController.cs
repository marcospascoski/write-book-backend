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
    [Route("api/acesso/perfis")]
    public class AcessoPerfilController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IPerfilAppService perfilAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        private readonly IPerfilAppService _perfilAppService = perfilAppService;

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Cadastrar(PerfilViewModel model)
        {
            return await TryExecuteAsync(_perfilAppService.Cadastrar(model));
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Alterar(PerfilViewModel model)
        {
            return await TryExecuteNoResultAsync(_perfilAppService.Alterar(model));
        }

        [HttpGet, Route("{id}")]
        [Authorize(Policy = "ClienteOnly")]
        public async Task<IActionResult> PesquisarPorId(long id)
        {
            return await TryExecuteAsync(_perfilAppService.PesquisarPorId(id));
        }

        [HttpPatch, Route("status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> AlterarStatus(PerfilAlterarStatusViewModel<long> model)
        {
            return await TryExecuteNoResultAsync(_perfilAppService.AlterarStatus(model));
        }

        [HttpPost, Route("paginar")]
        [Authorize(Policy = "ClienteOnly")]
        public async Task<IActionResult> Paginar(FiltroPerfilViewModel model)
        {
            return await TryExecuteAsync(_perfilAppService.Paginar(model));
        }
    }
}