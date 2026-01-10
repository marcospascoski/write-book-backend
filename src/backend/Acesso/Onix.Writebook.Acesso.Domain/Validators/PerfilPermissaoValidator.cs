using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Implementation;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Validators
{
    public class PerfilPermissaoValidator : BaseValidator<PerfilPermissao>, IPerfilPermissaoValidator
    {
        private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
        private readonly IPerfilRepository _perfilRepository;
        private readonly IPermissaoRepository _permissaoRepository;
        private readonly IStringLocalizer<Core.Resources.TextResource> _stringLocalizer;
        public PerfilPermissaoValidator(
            INotificationContext notificationContext,
            IPerfilPermissaoRepository perfilPermissaoRepository,
            IPerfilRepository perfilRepository,
            IPermissaoRepository permissaoRepository,
            IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _perfilPermissaoRepository = perfilPermissaoRepository;
            _perfilRepository = perfilRepository;
            _permissaoRepository = permissaoRepository;
            _stringLocalizer = stringLocalizer;

        }

        public async Task<bool> IsValid(PerfilPermissao perfil)
        {
            if (perfil == null)
            {
                return false;
            }
            
            // Validar se o Perfil existe
            var perfilExiste = await _perfilRepository.PesquisarPorIdAsync(perfil.PerfilId);
            if (perfilExiste == null)
            {
                var perfilString = _stringLocalizer.GetString("Perfil");
                NotificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
            }
            
            // Validar se a Permissão existe
            var permissaoExiste = await _permissaoRepository.PesquisarPorIdAsync(perfil.PermissaoId);
            if (permissaoExiste == null)
            {
                var permissaoString = _stringLocalizer.GetString("Permissao");
                NotificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
            }
            
            if (await _perfilPermissaoRepository.JaCadastrado(perfil.PerfilId, perfil.PermissaoId))
            {
                NotificationContext.AddError(_stringLocalizer.GetString("ErroPerfilPermissaoJaCadatrado"));
            }

            var validationResults = Validate(perfil);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
