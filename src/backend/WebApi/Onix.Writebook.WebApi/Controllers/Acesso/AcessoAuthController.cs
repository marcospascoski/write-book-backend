using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Onix.Framework.Domain.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.Security;
using Onix.Framework.WebApi.Controllers;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Login;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Onix.Writebook.WebApi.Controllers.Acesso
{
    [Route("api/auth")]
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

        /// <summary>
        /// Renova o Access Token usando o Refresh Token
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestViewModel request)
        {
            return await TryExecuteAsync(_authAppService.RefreshToken(request.RefreshToken));
        }

        /// <summary>
        /// Revoga um Refresh Token específico
        /// </summary>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequestViewModel request)
        {
            return await TryExecuteNoResultAsync(_authAppService.RevokeRefreshToken(request.RefreshToken));
        }

        /// <summary>
        /// Revoga todos os Refresh Tokens do usuário (logout de todos os dispositivos)
        /// </summary>
        [HttpPost("revoke-all")]
        [Authorize]
        public async Task<IActionResult> RevokeAllRefreshTokens()
        {
            var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || string.IsNullOrEmpty(usuarioIdClaim.Value))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(usuarioIdClaim.Value, out var usuarioId))
            {
                return BadRequest();
            }

            return await TryExecuteNoResultAsync(_authAppService.RevokeAllRefreshTokens(usuarioId));
        }
    }
}