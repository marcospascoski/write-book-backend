using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class PerfilPermissaoAppService(
        INotificationContext notificationContext,
        IAcessosUnitOfWork acessoUnitOfWork,
        IPerfilPermissaoRepository perfilPermissaoRepository,
        IPerfilPermissaoValidator perfilPermissaoValidator,
        IMapper mapper,
        IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        : IPerfilPermissaoAppService
    {
        public async Task<long> Cadastrar(PerfilPermissaoViewModel perfilPermissaoViewModel)
        {
            var prototype = mapper.Map<PerfilPermissao>(perfilPermissaoViewModel);
            var perfilPermissao = PerfilPermissao.Factory.Create(prototype);
            if (await perfilPermissaoValidator.IsValid(perfilPermissao))
            {
                await perfilPermissaoRepository.Cadastrar(perfilPermissao);
                await acessoUnitOfWork.CommitAsync();
                notificationContext.AddSuccess(stringLocalizer.GetString("SucessoCriarPerfilPermissao"));
                return perfilPermissao.Id;
            }
            return default;
        }

        public async Task Remover(PerfilPermissaoViewModel perfilPermissaoViewModel)
        {
            var perfilPermissao = await perfilPermissaoRepository.PesquisarPorIdAsync(perfilPermissaoViewModel.Id);

            if (perfilPermissao == null)
            {
                var perfilPermissaoString = stringLocalizer.GetString("PerfilPermissao");
                notificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", perfilPermissaoString));
                return;
            }

            perfilPermissaoRepository.Remover(perfilPermissao);
            await acessoUnitOfWork.CommitAsync();
            notificationContext.AddSuccess(stringLocalizer.GetString("SucessoRemoverPerfilPermissao"));
        }

        public async Task<PerfilPermissaoViewModel> PesquisarPorId(long id)
        {
            var perfilPermissao = await perfilPermissaoRepository.PesquisarPorIdAsync(id);
            var perfilPermissaoViewModel = mapper.Map<PerfilPermissaoViewModel>(perfilPermissao);
            return perfilPermissaoViewModel;
        }
    }
}
