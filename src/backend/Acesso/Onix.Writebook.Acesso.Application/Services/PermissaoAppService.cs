using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Infra.Data.Implementation;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class PermissaoAppService(
        INotificationContext notificationContext,
        IAcessosUnitOfWork acessoUnitOfWork,
        IPermissaoRepository permissaoRepository,
        IPermissaoValidator permissaoValidator,
        IMapper mapper,
        IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        : IPermissaoAppService
    {

        public async Task<long> Cadastrar(PermissaoViewModel permissaoViewModel)
        {
            var prototype = mapper.Map<Permissao>(permissaoViewModel);
            var permissao = Permissao.Factory.Create(prototype);
            if (await permissaoValidator.IsValid(permissao))
            {
                await permissaoRepository.Cadastrar(permissao);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoCriarPermissao"));
                return permissao.Id;
            }
            return default;
        }

        public async Task Alterar(PermissaoViewModel permissaoViewModel)
        {
            var permissao = await permissaoRepository.PesquisarPorIdAsync(permissaoViewModel.Id);
            if (permissao == null)
            {
                var permissaoString = stringLocalizer.GetString("Permissao");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
                return;
            }
            var permissaoAlterarDados = mapper.Map<Permissao>(permissaoViewModel);
            permissao.AlterarDados(permissaoAlterarDados);
            if (await permissaoValidator.IsValid(permissao))
            {
                permissaoRepository.Alterar(permissao);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosPermissao"));
            }
        }

        public async Task AlterarStatus(PermissaoAlterarStatusViewModel<long> alterarStatusViewModel)
        {
            var permissao = await permissaoRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (permissao == null)
            {
                var permissaoString = stringLocalizer.GetString("Permissao");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
                return;
            }
            var status = mapper.Map<EStatusEntidade>(alterarStatusViewModel.Status);
            permissao.AlterarStatus(status);
            if (await permissaoValidator.IsValid(permissao))
            {
                permissaoRepository.Alterar(permissao);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoAlterarDadosPermissao"));
            }
        }
        public async Task<IPagedItems<PermissaoViewModel>> Paginar(FiltroPermissaoViewModel model)
        {
            var pagedItems = await permissaoRepository.Paginar(model, model.Texto);
            return mapper.Map<PagedItems<PermissaoViewModel>>(pagedItems);
        }

        public async Task<PermissaoViewModel> PesquisarPorId(long id)
        {
            var permissao = await permissaoRepository.PesquisarPorIdAsync(id);
            var permissaoViewModel = mapper.Map<PermissaoViewModel>(permissao);
            return permissaoViewModel;
        }
    }
}
