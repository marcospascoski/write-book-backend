using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/perfilpermissoes")]
    public class AcessoPerfilPermissaoController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IPerfilPermissaoAppService perfilPermissaoAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        private readonly IPerfilPermissaoAppService _perfilPermissaoAppService = perfilPermissaoAppService;


        [HttpPost]
        public async Task<IActionResult> Cadastrar(PerfilPermissaoViewModel model)
        {
            return await TryExecuteAsync(_perfilPermissaoAppService.Cadastrar(model));
        }

        [HttpDelete]
        public async Task<IActionResult> Remover(PerfilPermissaoViewModel model)
        {
            return await TryExecuteNoResultAsync(_perfilPermissaoAppService.Remover(model));
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> PesquisarPorId(long id)
        {
            return await TryExecuteAsync(_perfilPermissaoAppService.PesquisarPorId(id));
        }

    }
}