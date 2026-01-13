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
        public async Task<Guid> CadastrarAsync(RegistrarUsuarioViewModel model)
        {
            var prototype = mapper.Map<Usuario>(model);
            if (prototype == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ErroEntidadeInvalida", usuarioString));
                return default;
            }

            var usuario = Usuario.Factory.Create(prototype);
            if (await usuarioValidator.IsValid(usuario))
            {
                await usuarioRepository.Cadastrar(usuario);
                await acessosUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoCriarUsuario"));
                
                await EnviarEmailConfirmacaoAsync(usuario.Id);
                
                return usuario.Id;
            }
            return default;
        }

        public async Task Alterar(UsuarioViewModel usuarioViewModel)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(usuarioViewModel.Id);
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }
            var usuarioAlterarDados = mapper.Map<Usuario>(usuarioViewModel);
            usuario.AlterarDados(usuarioAlterarDados);
            if (await usuarioValidator.IsValid(usuario))
            {
                usuarioRepository.Alterar(usuario);
                await acessosUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosUsuario"));
            }
        }

        public async Task<UsuarioViewModel> ObterPorEmailAsync(string email)
        {
            var usuario = await usuarioRepository.PesquisarPorEmailAsync(email);
            
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return null;
            }

            return mapper.Map<UsuarioViewModel>(usuario);
        }

        public async Task<UsuarioViewModel> PesquisarPorIdAsync(Guid id)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(id);
            
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return null;
            }

            return mapper.Map<UsuarioViewModel>(usuario);
        }

        public async Task AlterarStatus(UsuarioAlterarStatusViewModel alterarStatusViewModel)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }
            var status = mapper.Map<EStatusUsuario>(alterarStatusViewModel.Status);
            usuario.AlterarStatus(status);
            if (await usuarioValidator.IsValid(usuario))
            {
                usuarioRepository.Alterar(usuario);
                await acessosUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosUsuario"));
            }
        }

        public async Task<IPagedItems<UsuarioViewModel>> Paginar(FiltroUsuarioViewModel model)
        {
            var pagedItems = await usuarioRepository.Paginar(model, model.Texto);
            return mapper.Map<PagedItems<UsuarioViewModel>>(pagedItems);
        }

        public async Task<UsuarioViewModel> PesquisarPorId(Guid id)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(id);
            var usuarioViewModel = mapper.Map<UsuarioViewModel>(usuario);
            if (usuarioViewModel != null) usuarioViewModel.Senha = null;
            return usuarioViewModel;
        }

        public async Task<bool> AlterarPerfilAsync(Guid usuarioId, UsuarioViewModel updates)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(usuarioId);
            if (usuario == null) return false;

            // No mock o perfil atualiza avatar, nome, etc.
            var dadosAtualizar = mapper.Map<Usuario>(updates);
            usuario.AlterarDados(dadosAtualizar);

            usuarioRepository.Alterar(usuario);
            return await acessosUnitOfWork.CommitAsync() > 0;
        }

        public async Task RedefinirSenhaAsync(UsuarioRedefinirSenhaViewModel redefinirSenhaViewModel)
        {
            // Busca o token
            var tokenRedefinicaoSenha = await tokenRedefinicaoSenhaRepository.PesquisarPorTokenAsync(redefinirSenhaViewModel.Token);
            
            if (tokenRedefinicaoSenha == null)
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroTokenRedefinicaoInvalido"));
                return;
            }

            // Valida o token
            if (!tokenRedefinicaoSenha.EstaValido())
            {
                if (tokenRedefinicaoSenha.EstaExpirado())
                {
                    notificationContext.AddError(stringLocalizer.GetString("ErroTokenRedefinicaoExpirado"));
                }
                else
                {
                    notificationContext.AddError(stringLocalizer.GetString("ErroTokenRedefinicaoUtilizado"));
                }
                return;
            }

            // Busca o usuário
            var usuario = await usuarioRepository.PesquisarPorIdAsync(tokenRedefinicaoSenha.UsuarioId);
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return;
            }

            // Redefine a senha
            usuario.RedefinirSenha(redefinirSenhaViewModel.NovaSenha);
            
            if (await usuarioValidator.IsValid(usuario))
            {
                // Marca o token como utilizado
                tokenRedefinicaoSenha.MarcarComoUtilizado();
                tokenRedefinicaoSenhaRepository.Alterar(tokenRedefinicaoSenha);
                
                // Atualiza o usuário
                usuarioRepository.Alterar(usuario);
                await acessosUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoSenhaRedefinida"));
            }
        }

        public async Task<bool> EnviarEmailConfirmacaoAsync(Guid usuarioId)
        {
            var usuario = await usuarioRepository.PesquisarPorIdAsync(usuarioId);
            if (usuario == null)
            {
                var usuarioString = stringLocalizer.GetString("Usuario");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", usuarioString));
                return false;
            }

            var tokenConfirmacao = Guid.NewGuid().ToString();
            var emailConfirmacaoViewModel = mapper.Map<EmailConfirmacaoViewModel>(usuario, opts =>
            {
                opts.Items["TokenConfirmacao"] = tokenConfirmacao;
            });
            
            var emailEnviado = await emailAppService.EnviarEmailConfirmacaoAsync(emailConfirmacaoViewModel);

            if (emailEnviado)
            {
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoEmailConfirmacaoEnviado"));
            }
            else
            {
                notificationContext.AddError(stringLocalizer.GetString("ErroEmailConfirmacao"));
            }

            return emailEnviado;
        }

        public async Task<bool> SolicitarRedefinicaoSenhaAsync(SolicitarRedefinicaoSenhaViewModel model)
        {
            var usuario = await usuarioRepository.PesquisarPorEmailAsync(model.Email);
            if (usuario == null)
            {
                // Por segurança, não informamos se o email existe ou não
                notificationContext.AddSuccess(stringLocalizer.GetString("MensagemSolicitacaoRedefinicao"));
                return true;
            }

            await tokenRedefinicaoSenhaRepository.InvalidarTokensAntigos(usuario.Id);
            
            var tokenRedefinicaoSenha = new TokenRedefinicaoSenha(usuario.Id);
            if (await tokenRedefinicaoSenhaValidator.IsValid(tokenRedefinicaoSenha))
            {
                await tokenRedefinicaoSenhaRepository.Cadastrar(tokenRedefinicaoSenha);
                await acessosUnitOfWork.CommitAsync();
                var emailRedefinicaoViewModel = mapper.Map<EmailRedefinicaoSenhaViewModel>(usuario, opts =>
                {
                    opts.Items["TokenRedefinicao"] = tokenRedefinicaoSenha.Token;
                });

                var emailEnviado = await emailAppService.EnviarEmailRedefinicaoSenhaAsync(emailRedefinicaoViewModel);
                if (emailEnviado)
                {
                    notificationContext.AddSuccess(stringLocalizer.GetString("SucessoEmailRedefinicaoEnviado"));
                }
                else
                {
                    notificationContext.AddError(stringLocalizer.GetString("ErroEmailRedefinicao"));
                }
                return emailEnviado;
            }
            return false;
        }
    }
}
