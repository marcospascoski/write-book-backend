using FluentValidation;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using Onix.Writebook.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Validators
{
    public class PerfilValidator : BaseValidator<Perfil>, IPerfilValidator
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly IStringLocalizer<TextResource> _stringLocalizer;
        public PerfilValidator(
            INotificationContext notificationContext,
            IPerfilRepository perfilRepository,
            IStringLocalizer<TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _perfilRepository = perfilRepository;
            _stringLocalizer = stringLocalizer;

            RuleFor(x => x.Nome)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(128)
                .WithName(stringLocalizer.GetString("Nome"));
        }

        public async Task<bool> IsValid(Perfil perfil)
        {
            if (perfil == null)
            {
                return false;
            }
            if (await _perfilRepository.JaCadastrado(perfil.Nome, perfil.Id))
            {
                NotificationContext.AddError(_stringLocalizer.GetString("ErroPerfilJaCadatradoNome"));
            }

            var validationResults = Validate(perfil);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
