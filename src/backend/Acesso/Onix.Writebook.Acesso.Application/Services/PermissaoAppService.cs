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
    public class PermissaoAppService : IPermissaoAppService
    {
        private readonly INotificationContext _notificationContext;
        private readonly IAcessosUnitOfWork _acessoUnitOfWork;
        private readonly IPermissaoRepository _permissaoRepository;
        private readonly IPermissaoValidator _permissaoValidator;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Core.Resources.TextResource> _stringLocalizer;

        public PermissaoAppService(
            INotificationContext notificationContext,
            IAcessosUnitOfWork acessoUnitOfWork,
            IPermissaoRepository permissaoRepository,
            IPermissaoValidator permissaoValidator,
            IMapper mapper,
            IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        {
            _notificationContext = notificationContext;
            _acessoUnitOfWork = acessoUnitOfWork;
            _permissaoRepository = permissaoRepository;
            _permissaoValidator = permissaoValidator;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<long> Cadastrar(PermissaoViewModel permissaoViewModel)
        {
            var prototype = _mapper.Map<Permissao>(permissaoViewModel);
            var permissao = Permissao.Factory.Create(prototype);
            if (await _permissaoValidator.IsValid(permissao))
            {
                await _permissaoRepository.Cadastrar(permissao);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoCriarPermissao"));
                return permissao.Id;
            }
            return default;
        }

        public async Task Alterar(PermissaoViewModel permissaoViewModel)
        {
            var permissao = await _permissaoRepository.PesquisarPorIdAsync(permissaoViewModel.Id);
            if (permissao == null)
            {
                var permissaoString = _stringLocalizer.GetString("Permissao");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
                return;
            }
            var permissaoAlterarDados = _mapper.Map<Permissao>(permissaoViewModel);
            permissao.AlterarDados(permissaoAlterarDados);
            if (await _permissaoValidator.IsValid(permissao))
            {
                _permissaoRepository.Alterar(permissao);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosPermissao"));
            }
        }

        public async Task AlterarStatus(PermissaoAlterarStatusViewModel<long> alterarStatusViewModel)
        {
            var permissao = await _permissaoRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (permissao == null)
            {
                var permissaoString = _stringLocalizer.GetString("Permissao");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
                return;
            }
            var status = _mapper.Map<EStatusEntidade>(alterarStatusViewModel.Status);
            permissao.AlterarStatus(status);
            if (await _permissaoValidator.IsValid(permissao))
            {
                _permissaoRepository.Alterar(permissao);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosPermissao"));
            }
        }
        public async Task<IPagedItems<PermissaoViewModel>> Paginar(FiltroPermissaoViewModel model)
        {
            var pagedItems = await _permissaoRepository.Paginar(model, model.Texto);
            return _mapper.Map<PagedItems<PermissaoViewModel>>(pagedItems);
        }

        public async Task<PermissaoViewModel> PesquisarPorId(long id)
        {
            var permissao = await _permissaoRepository.PesquisarPorIdAsync(id);
            var permissaoViewModel = _mapper.Map<PermissaoViewModel>(permissao);
            return permissaoViewModel;
        }
    }
}
