using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Framework.Security.JwtConfig;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Login;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Domain.Validators;
using Onix.Writebook.Acesso.Infra.Data.UnitOfWork;
using Onix.Writebook.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class AuthAppService(
        INotificationContext notificationContext,
        IUsuarioRepository usuarioRepository,
        IAcessosUnitOfWork acessosUnitOfWork,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenValidator refreshTokenValidator,
        ITokenBlacklistService tokenBlacklistService,
        IMapper mapper,
        IStringLocalizer<TextResource> stringLocalizer,
        IHttpClientInfoService httpClientInfoService)
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

            var ipAddress = httpClientInfoService.GetClientIpAddress();
            var userAgent = httpClientInfoService.GetUserAgent();

            await refreshTokenRepository.RevogarTokensUsuarioAsync(usuario.Id);

            var refreshToken = Domain.Entities.RefreshToken.Factory.Create(usuario.Id, ipAddress, userAgent);
            if (await refreshTokenValidator.IsValid(refreshToken))
            {

                await refreshTokenRepository.Cadastrar(refreshToken);
                await acessosUnitOfWork.CommitAsync();

                var usuarioViewModel = mapper.Map<UsuarioViewModel>(usuario);
                usuarioViewModel.AccessToken = accessToken;
                usuarioViewModel.RefreshToken = refreshToken.Token;

                return usuarioViewModel;
            }
            return default;
        }
        public async Task Logout(string token)
        {
            await tokenBlacklistService.CadastrarNaBlackListAsync(token);
        }

        public async Task<UsuarioViewModel> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenNulo"));
                return null;
            }

            var tokenEntity = await refreshTokenRepository.PesquisarPorTokenAsync(refreshToken);

            if (tokenEntity == null)
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenInvalido"));
                return null;
            }

            if (!tokenEntity.EstaValido())
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenExpiradoOuRevogado"));
                return null;
            }

            var usuario = await usuarioRepository.PesquisarPorIdAsync(tokenEntity.UsuarioId);

            if (usuario == null)
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroUsuarioNaoEncontrado"));
                return null;
            }

            // Gerar novo access token
            var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new(ClaimTypes.Email, usuario.Email),
                    };

            var newAccessToken = JwtConfig.GerarAccessToken(claims);

            // Revogar o refresh token usado (rotation)
            tokenEntity.Revogar();
            refreshTokenRepository.Alterar(tokenEntity);

            // Criar novo refresh token
            var ipAddress = httpClientInfoService.GetClientIpAddress();
            var userAgent = httpClientInfoService.GetUserAgent();

            var newRefreshToken = Domain.Entities.RefreshToken.Factory.Create(usuario.Id, ipAddress, userAgent);

            if (await refreshTokenValidator.IsValid(newRefreshToken))
            {
                await refreshTokenRepository.Cadastrar(newRefreshToken);
                await acessosUnitOfWork.CommitAsync();

                var usuarioViewModel = mapper.Map<UsuarioViewModel>(usuario);
                usuarioViewModel.AccessToken = newAccessToken;
                usuarioViewModel.RefreshToken = newRefreshToken.Token;

                return usuarioViewModel;
            }

            return null;
        }

        public async Task RevokeRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenNulo"));
                return;
            }

            var tokenEntity = await refreshTokenRepository.PesquisarPorTokenAsync(refreshToken);

            if (tokenEntity == null)
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenInvalido"));
                return;
            }

            tokenEntity.Revogar();
            refreshTokenRepository.Alterar(tokenEntity);
            await acessosUnitOfWork.CommitAsync();
        }

        public async Task RevokeAllRefreshTokens(Guid usuarioId)
        {
            await refreshTokenRepository.RevogarTokensUsuarioAsync(usuarioId);
            await acessosUnitOfWork.CommitAsync();
        }
    }
}