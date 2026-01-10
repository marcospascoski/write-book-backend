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
    public class PerfilPermissaoAppService : IPerfilPermissaoAppService
    {
        private readonly INotificationContext _notificationContext;
        private readonly IAcessosUnitOfWork _acessoUnitOfWork;
        private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
        private readonly IPerfilPermissaoValidator _perfilPermissaoValidator;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Core.Resources.TextResource> _stringLocalizer;

        public PerfilPermissaoAppService(
            INotificationContext notificationContext,
            IAcessosUnitOfWork acessoUnitOfWork,
            IPerfilPermissaoRepository perfilPermissaoRepository,
            IPerfilPermissaoValidator perfilPermissaoValidator,
            IMapper mapper,
            IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        {
            _notificationContext = notificationContext;
            _acessoUnitOfWork = acessoUnitOfWork;
            _perfilPermissaoRepository = perfilPermissaoRepository;
            _perfilPermissaoValidator = perfilPermissaoValidator;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<long> Cadastrar(PerfilPermissaoViewModel perfilPermissaoViewModel)
        {
            var prototype = _mapper.Map<PerfilPermissao>(perfilPermissaoViewModel);
            var perfilPermissao = PerfilPermissao.Factory.Create(prototype);
            if (await _perfilPermissaoValidator.IsValid(perfilPermissao))
            {
                await _perfilPermissaoRepository.Cadastrar(perfilPermissao);
                await _acessoUnitOfWork.CommitAsync();
                _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoCriarPerfilPermissao"));
                return perfilPermissao.Id;
            }
            return default;
        }

        public async Task Remover(PerfilPermissaoViewModel perfilPermissaoViewModel)
        {
            var perfilPermissao = await _perfilPermissaoRepository.PesquisarPorIdAsync(perfilPermissaoViewModel.Id);

            if (perfilPermissao == null)
            {
                var perfilPermissaoString = _stringLocalizer.GetString("PerfilPermissao");
                _notificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", perfilPermissaoString));
                return;
            }
            
            _perfilPermissaoRepository.Remover(perfilPermissao);
            await _acessoUnitOfWork.CommitAsync();
            _notificationContext.AddSuccess(_stringLocalizer.GetString("SucessoRemoverPerfilPermissao"));
        }
        public async Task<PerfilPermissaoViewModel> PesquisarPorId(long id)
        {
            var perfilPermissao = await _perfilPermissaoRepository.PesquisarPorIdAsync(id);
            var perfilPermissaoViewModel = _mapper.Map<PerfilPermissaoViewModel>(perfilPermissao);
            return perfilPermissaoViewModel;
        }
    }
}
