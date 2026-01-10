using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Login;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/acesso/auth")]
    public class AcessoAuthController(
        INotificationContext notificationContext,
        IExceptionProcessor exceptionProcessor,
        IAuthAppService authAppService)
        : BaseController(notificationContext, exceptionProcessor)
    {
        private readonly IAuthAppService _authAppService = authAppService;


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            return await TryExecuteAsync(_authAppService.Login(loginViewModel));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(string accessToken)
        {
            return await TryExecuteNoResultAsync(_authAppService.Logout(accessToken));
        }
    }
}