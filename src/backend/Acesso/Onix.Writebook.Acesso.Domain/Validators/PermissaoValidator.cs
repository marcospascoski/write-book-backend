using FluentValidation;
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
    public class PermissaoValidator : BaseValidator<Permissao>, IPermissaoValidator
    {
        private readonly IPermissaoRepository _perfilRepository;
        private readonly IStringLocalizer<Core.Resources.TextResource> _stringLocalizer;

        public PermissaoValidator(
            INotificationContext notificationContext,
            IPermissaoRepository perfilRepository,
            IStringLocalizer<Core.Resources.TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _perfilRepository = perfilRepository;
            _stringLocalizer = stringLocalizer;
            
            RuleFor(x => x.Nome)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(128)
                .WithName(_stringLocalizer.GetString("Nome"));
        }

        public async Task<bool> IsValid(Permissao perfil)
        {
            if (perfil == null)
            {
                return false;
            }
            if (await _perfilRepository.JaCadastrado(perfil.Nome, perfil.Id))
            {
                NotificationContext.AddError(_stringLocalizer.GetString("ErroPermissaoJaCadatradoNome"));
            }

            var validationResults = Validate(perfil);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
