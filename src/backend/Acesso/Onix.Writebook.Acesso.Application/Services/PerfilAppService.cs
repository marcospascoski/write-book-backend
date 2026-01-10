using AutoMapper;
using Microsoft.Extensions.Localization;
using Onix.Framework.Infra.Data.Implementation;
using Onix.Framework.Infra.Data.Interfaces;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
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
    public class PerfilAppService : IPerfilAppService
    {
        private readonly INotificationContext _notificationContext;
        private readonly IAcessosUnitOfWork _acessoUnitOfWork;
        private readonly IPerfilRepository _perfilRepository;
        private readonly IPerfilValidator _perfilValidator;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Core.Resources.TextResource> _stringLocalizer;

        public PerfilAppService(
            INotificationContext notificationContext,
            IAcessosUnitOfWork acessoUnitOfWork,
            IPerfilRepository perfilRepository,
        IPerfilValidator perfilValidator,
            IMapper mapper,
            IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        {
            _notificationContext = notificationContext;
            _acessoUnitOfWork = acessoUnitOfWork;
            _perfilRepository = perfilRepository;
            _perfilValidator = perfilValidator;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<long> Cadastrar(PerfilViewModel perfilViewModel)
        {
            var prototype = _mapper.Map<Perfil>(perfilViewModel);
            var perfil = Perfil.Factory.Create(prototype);
            if (await _perfilValidator.IsValid(perfil))
            {
                await _perfilRepository.Cadastrar(perfil);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoCriarPerfil"));
                return perfil.Id;
            }
            return default;
        }

        public async Task Alterar(PerfilViewModel perfilViewModel)
        {
            var perfil = await _perfilRepository.PesquisarPorIdAsync(perfilViewModel.Id);
            if (perfil == null)
            {
                var perfilString = _stringLocalizer.GetString("Perfil");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
                return;
            }
            var perfilAlterarDados = _mapper.Map<Perfil>(perfilViewModel);
            perfil.AlterarDados(perfilAlterarDados);
            if (await _perfilValidator.IsValid(perfil))
            {
                _perfilRepository.Alterar(perfil);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosPerfil"));
            }
        }

        public async Task AlterarStatus(PerfilAlterarStatusViewModel<long> alterarStatusViewModel)
        {
            var perfil = await _perfilRepository.PesquisarPorIdAsync(alterarStatusViewModel.Id);
            if (perfil == null)
            {
                var perfilString = _stringLocalizer.GetString("Perfil");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
                return;
            }
            var status = _mapper.Map<EStatusEntidade>(alterarStatusViewModel.Status);
            perfil.AlterarStatus(status);
            if (await _perfilValidator.IsValid(perfil))
            {
                _perfilRepository.Alterar(perfil);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoAlterarDadosPerfil"));
            }
        }
        public async Task<IPagedItems<PerfilViewModel>> Paginar(FiltroPerfilViewModel model)
        {
            var pagedItems = await _perfilRepository.Paginar(model, model.Texto);
            return _mapper.Map<PagedItems<PerfilViewModel>>(pagedItems);
        }

        public async Task<PerfilViewModel> PesquisarPorId(long id)
        {
            var perfil = await _perfilRepository.PesquisarPorIdAsync(id);
            var perfilViewModel = _mapper.Map<PerfilViewModel>(perfil);
            return perfilViewModel;
        }
    }
}
