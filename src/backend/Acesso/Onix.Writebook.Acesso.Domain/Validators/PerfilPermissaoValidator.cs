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
    public class PerfilPermissaoValidator(
        INotificationContext notificationContext,
        IPerfilPermissaoRepository perfilPermissaoRepository,
        IPerfilRepository perfilRepository,
        IPermissaoRepository permissaoRepository,
        IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
        : BaseValidator<PerfilPermissao>(notificationContext), IPerfilPermissaoValidator
    {
        public async Task<bool> IsValid(PerfilPermissao perfil)
        {
            if (perfil == null)
            {
                return false;
            }

            // Validar se o Perfil existe
            var perfilExiste = await perfilRepository.PesquisarPorIdAsync(perfil.PerfilId);
            if (perfilExiste == null)
            {
                var perfilString = stringLocalizer.GetString("Perfil");
                NotificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", perfilString));
            }

            // Validar se a Permissão existe
            var permissaoExiste = await permissaoRepository.PesquisarPorIdAsync(perfil.PermissaoId);
            if (permissaoExiste == null)
            {
                var permissaoString = stringLocalizer.GetString("Permissao");
                NotificationContext.AddError(stringLocalizer.GetString("ObjetoNaoEncontrado", permissaoString));
            }

            if (await perfilPermissaoRepository.JaCadastrado(perfil.PerfilId, perfil.PermissaoId))
            {
                NotificationContext.AddError(stringLocalizer.GetString("ErroPerfilPermissaoJaCadatrado"));
            }

            var validationResults = Validate(perfil);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
