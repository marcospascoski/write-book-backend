using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.Security.JwtConfig;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Login;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Resources;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class AuthAppService(
        INotificationContext notificationContext,
        IUsuarioRepository usuarioRepository,
        IAcessosUnitOfWork acessosUnitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenBlacklistService tokenBlacklistService,
        IMapper mapper,
        IStringLocalizer<TextResource> stringLocalizer)
        : IAuthAppService
    {
        public async Task<UsuarioViewModel> Login(LoginViewModel loginViewModel)
        {
            var usuario = await usuarioRepository.PesquisarPorEmailAsync(loginViewModel.Email);
            if (usuario == null)
            {
                var UsuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", UsuarioString));
                return null;
            }

            if (!usuario.Senha.ValidarSenha(loginViewModel.Senha, usuario.Salt.Valor))
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroSenhaInvalida"));
                return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Email, usuario.Email),
            };

            var accessToken = JwtConfig.GerarAccessToken(claims);

            if (await tokenBlacklistService.EstaNaBlackListAsync(accessToken))
            {
                notificationContext.AddError(stringLocalizer["TokenInvalido"]);
                return null;
            }

            // Create a new refresh token for the user
            var refreshToken = new RefreshToken(usuario.Id, ipAddress: "0.0.0.0", userAgent: "Unknown");
            await refreshTokenRepository.Cadastrar(refreshToken);

            await acessosUnitOfWork.CommitAsync();

            var usuarioViewModel = mapper.Map<UsuarioViewModel>(usuario);
            usuarioViewModel.AccessToken = accessToken;
            usuarioViewModel.RefreshToken = refreshToken.Token;

            return usuarioViewModel;
        }

        public async Task Logout(string token)
        {
            await tokenBlacklistService.CadastrarNaBlackListAsync(token);
        }
    }
}
