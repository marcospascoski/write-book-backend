using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Infra.Data.Implementation;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Enums;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Application.Interfaces;
using Onix.Writebook.Core.Application.ViewModels;
using Onix.Writebook.Core.Domain.Enums;
using Onix.Writebook.Core.Resources;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class UsuarioAppService(
        INotificationContext notificationContext,
        IAcessosUnitOfWork acessosUnitOfWork,
        IUsuarioRepository usuarioRepository,
        IUsuarioValidator usuarioValidator,
        IMapper mapper,
        IStringLocalizer<TextResource> stringLocalizer,
        IEmailAppService emailAppService,
        ITokenRedefinicaoSenhaRepository tokenRedefinicaoSenhaRepository,
        ITokenRedefinicaoSenhaValidator tokenRedefinicaoSenhaValidator) : IUsuarioAppService
    {
        private readonly INotificationContext _notificationContext = notificationContext;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork = acessosUnitOfWork;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioValidator _usuarioValidator = usuarioValidator;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<TextResource> _stringLocalizer = stringLocalizer;
        private readonly IEmailAppService _emailAppService = emailAppService;
        private readonly ITokenRedefinicaoSenhaRepository _tokenRedefinicaoSenhaRepository = tokenRedefinicaoSenhaRepository;
        private readonly ITokenRedefinicaoSenhaValidator _tokenRedefinicaoSenhaValidator = tokenRedefinicaoSenhaValidator;

        public async Task<bool> CreditarRecompensa(Guid usuarioId, decimal valor)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioId);
            if (usuario == null) return false;

            // No sistema atual não persistimos saldos financeiros diretamente no módulo de Acesso.
            // Marca apenas a modificação e persiste para fins de mock/registro.
            usuario.AlterarDados(usuario);
            _usuarioRepository.Alterar(usuario);
            return await _acessosUnitOfWork.CommitAsync() > 0;
        }

        public async Task<Guid> CadastrarAsync(RegistrarUsuarioViewModel model)
        {
            var prototype = _mapper.Map<Usuario>(model);
            if (prototype == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ErroEntidadeInvalida", usuarioString));
                return default;
            }

            var usuario = Usuario.Factory.Create(prototype);
            if (await _usuarioValidator.IsValid(usuario))
            {
                await _usuarioRepository.Cadastrar(usuario);
                await _acessosUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoCriarUsuario"));
                
                await EnviarEmailConfirmacaoAsync(usuario.Id);
                
                return usuario.Id;
            }
            return default;
        }

        public async Task Alterar(UsuarioViewModel usuarioViewModel)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioViewModel.Id);
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }
            var usuarioAlterarDados = _mapper.Map<Usuario>(usuarioViewModel);
            usuario.AlterarDados(usuarioAlterarDados);
            if (await _usuarioValidator.IsValid(usuario))
            {
                _usuarioRepository.Alterar(usuario);
                await _acessosUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosUsuario"));
            }
        }

        public async Task<UsuarioViewModel> ObterPorEmailAsync(string email)
        {
            var usuario = await _usuarioRepository.PesquisarPorEmailAsync(email);
            
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return null;
            }

            return _mapper.Map<UsuarioViewModel>(usuario);
        }

        public async Task<UsuarioViewModel> PesquisarPorIdAsync(Guid id)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(id);
            
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return null;
            }

            return _mapper.Map<UsuarioViewModel>(usuario);
        }

        public async Task AlterarStatus(UsuarioAlterarStatusViewModel alterarStatusViewModel)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }
            var status = _mapper.Map<EStatusUsuario>(alterarStatusViewModel.Status);
            usuario.AlterarStatus(status);
            if (await _usuarioValidator.IsValid(usuario))
            {
                _usuarioRepository.Alterar(usuario);
                await _acessosUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosUsuario"));
            }
        }

        public async Task<IPagedItems<UsuarioViewModel>> Paginar(FiltroUsuarioViewModel model)
        {
            var pagedItems = await _usuarioRepository.Paginar(model, model.Texto);
            return _mapper.Map<PagedItems<UsuarioViewModel>>(pagedItems);
        }

        public async Task<UsuarioViewModel> PesquisarPorId(Guid id)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(id);
            var usuarioViewModel = _mapper.Map<UsuarioViewModel>(usuario);
            if (usuarioViewModel != null) usuarioViewModel.Senha = null;
            return usuarioViewModel;
        }

        public async Task<bool> AlterarPerfilAsync(Guid usuarioId, UsuarioViewModel updates)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioId);
            if (usuario == null) return false;

            // No mock o perfil atualiza avatar, nome, etc.
            var dadosAtualizar = _mapper.Map<Usuario>(updates);
            usuario.AlterarDados(dadosAtualizar);

            _usuarioRepository.Alterar(usuario);
            return await _acessosUnitOfWork.CommitAsync() > 0;
        }

        public async Task RedefinirSenhaAsync(UsuarioRedefinirSenhaViewModel redefinirSenhaViewModel)
        {
            // Busca o token
            var tokenRedefinicaoSenha = await _tokenRedefinicaoSenhaRepository.PesquisarPorTokenAsync(redefinirSenhaViewModel.Token);
            
            if (tokenRedefinicaoSenha == null)
            {
                _notificationContext.AddError(_stringLocalizer.GetString("ErroTokenRedefinicaoInvalido"));
                return;
            }

            // Valida o token
            if (!tokenRedefinicaoSenha.EstaValido())
            {
                if (tokenRedefinicaoSenha.EstaExpirado())
                {
                    _notificationContext.AddError(_stringLocalizer.GetString("ErroTokenRedefinicaoExpirado"));
                }
                else
                {
                    _notificationContext.AddError(_stringLocalizer.GetString("ErroTokenRedefinicaoUtilizado"));
                }
                return;
            }

            // Busca o usuário
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(tokenRedefinicaoSenha.UsuarioId);
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }

            // Redefine a senha
            usuario.RedefinirSenha(redefinirSenhaViewModel.NovaSenha);
            
            if (await _usuarioValidator.IsValid(usuario))
            {
                // Marca o token como utilizado
                tokenRedefinicaoSenha.MarcarComoUtilizado();
                _tokenRedefinicaoSenhaRepository.Alterar(tokenRedefinicaoSenha);
                
                // Atualiza o usuário
                _usuarioRepository.Alterar(usuario);
                await _acessosUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoSenhaRedefinida"));
            }
        }

        public async Task<bool> EnviarEmailConfirmacaoAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.PesquisarPorIdAsync(usuarioId);
            if (usuario == null)
            {
                var usuarioString = _stringLocalizer.GetString("Usuario");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return false;
            }

            var tokenConfirmacao = Guid.NewGuid().ToString();
            var emailConfirmacaoViewModel = _mapper.Map<EmailConfirmacaoViewModel>(usuario, opts =>
            {
                opts.Items["TokenConfirmacao"] = tokenConfirmacao;
            });
            
            var emailEnviado = await _emailAppService.EnviarEmailConfirmacaoAsync(emailConfirmacaoViewModel);

            if (emailEnviado)
            {
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoEmailConfirmacaoEnviado"));
            }
            else
            {
                _notificationContext.AddError(_stringLocalizer.GetString("ErroEmailConfirmacao"));
            }

            return emailEnviado;
        }

        public async Task<bool> SolicitarRedefinicaoSenhaAsync(SolicitarRedefinicaoSenhaViewModel model)
        {
            var usuario = await _usuarioRepository.PesquisarPorEmailAsync(model.Email);
            if (usuario == null)
            {
                // Por segurança, não informamos se o email existe ou não
                _notificationContext.AddSuccess(_stringLocalizer.GetString("MensagemSolicitacaoRedefinicao"));
                return true;
            }

            await _tokenRedefinicaoSenhaRepository.InvalidarTokensAntigos(usuario.Id);
            
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id);
            if (await _tokenRedefinicaoSenhaValidator.IsValid(tokenRedefinicaoSenha))
            {
                await _tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
                await _acessosUnitOfWork.CommitAsync();
                var emailRedefinicaoViewModel = _mapper.Map<EmailRedefinicaoSenhaViewModel>(usuario, opts =>
                {
                    opts.Items["TokenRedefinicao"] = tokenRedefinicaoSenha.Token;
                });

                var emailEnviado = await _emailAppService.EnviarEmailRedefinicaoSenhaAsync(emailRedefinicaoViewModel);
                if (emailEnviado)
                {
                    _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoEmailRedefinicaoEnviado"));
                }
                else
                {
                    _notificationContext.AddError(_stringLocalizer.GetString("ErroEmailRedefinicao"));
                }
                return emailEnviado;
            }
            return false;
        }
    }
}
