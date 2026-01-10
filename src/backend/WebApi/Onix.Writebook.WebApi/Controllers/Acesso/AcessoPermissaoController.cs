using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/permissoes")]
    public class AcessoPermissaoController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IPermissaoAppService permissaoAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        private readonly IPermissaoAppService _permissaoAppService = permissaoAppService;


        [HttpPost]
        public async Task<IActionResult> Cadastrar(PermissaoViewModel model)
        {
            return await TryExecuteAsync(_permissaoAppService.Cadastrar(model));
        }

        [HttpPut]
        public async Task<IActionResult> Alterar(PermissaoViewModel model)
        {
            return await TryExecuteNoResultAsync(_permissaoAppService.Alterar(model));
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> PesquisarPorId(long id)
        {
            return await TryExecuteAsync(_permissaoAppService.PesquisarPorId(id));
        }

        [HttpPatch, Route("status")]
        public async Task<IActionResult> AlterarStatus(PermissaoAlterarStatusViewModel<long> model)
        {
            return await TryExecuteNoResultAsync(_permissaoAppService.AlterarStatus(model));
        }

        [HttpPost, Route("paginar")]
        public async Task<IActionResult> Paginar(FiltroPermissaoViewModel model)
        {
            return await TryExecuteAsync(_permissaoAppService.Paginar(model));
        }
    }
}