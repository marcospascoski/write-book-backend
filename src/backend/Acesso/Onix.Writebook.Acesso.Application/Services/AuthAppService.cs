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
        ITokenBlacklistService tokenBlacklistService,
        IMapper mapper,
        IStringLocalizer<TextResource> stringLocalizer)
        : IAuthAppService
    {
        private readonly INotificationContext _notificationContext = notificationContext;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork = acessosUnitOfWork;
        private readonly ITokenBlacklistService _tokenBlacklistService = tokenBlacklistService;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<TextResource> _stringLocalizer = stringLocalizer;

        public async Task<UsuarioViewModel> Login(LoginViewModel loginViewModel)
        {
            var usuario = await _usuarioRepository.PesquisarPorEmailAsync(loginViewModel.Email);
            if (usuario == null)
            {
                var UsuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", UsuarioString));
                return null;
            }

            if (!usuario.Senha.ValidarSenha(loginViewModel.Senha, usuario.Salt.Valor))
            {
                _notificationContext.AddError(_stringLocalizer.GetString("ErroSenhaInvalida"));
                return null;
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Email, usuario.Email),
            };

            var accessToken = JwtConfig.GerarAccessToken(claims);

            if (await _tokenBlacklistService.EstaNaBlackListAsync(accessToken))
            {
                _notificationContext.AddError(_stringLocalizer["TokenInvalido"]);
                return null;
            }

            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            usuarioViewModel.Senha = null;
            usuarioViewModel.AccessToken = accessToken;

            return usuarioViewModel;
        }

        public Task Logout(string accessToken)
        {
            return Task.CompletedTask;
        }
    }
}
