using FluentValidation;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using Onix.Writebook.Core.Resources;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Validators
{
    public class RefreshTokenValidator : BaseValidator<RefreshToken>, IRefreshTokenValidator
    {
        private readonly IStringLocalizer<TextResource> _stringLocalizer;

        public RefreshTokenValidator(
            INotificationContext notificationContext,
            IStringLocalizer<TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _stringLocalizer = stringLocalizer;

            RuleFor(x => x.UsuarioId)
                            .NotEqual(Guid.Empty)
                            .WithMessage(_stringLocalizer?.GetString("ErroTokenUsuarioObrigatorio"));

            RuleFor(x => x.Token)
                .NotEmpty()
                .Length(32)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenFormatoInvalido"));

            RuleFor(x => x.CreatedAt)
                .GreaterThan(DateTime.MinValue)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataCriacao"));

            RuleFor(x => x.DataExpiracao)
                .GreaterThan(x => x.CreatedAt)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataExpiracao"));

            RuleFor(x => x)
                .Must(t => !t.Revogado || t.DataRevogacao.HasValue)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenRevogadoSemData"));
        }

        public async Task<bool> IsValid(RefreshToken token)
        {
            if (token == null)
            {
                return false;
            }

            var validationResults = Validate(token);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}