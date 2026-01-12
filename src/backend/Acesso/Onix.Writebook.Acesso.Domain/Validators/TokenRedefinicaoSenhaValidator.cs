using FluentValidation;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Validators
{
    public class TokenRedefinicaoSenhaValidator : BaseValidator<TokenRedefinicaoSenha>, ITokenRedefinicaoSenhaValidator
    {
        public TokenRedefinicaoSenhaValidator(INotificationContext notificationContext)
            : base(notificationContext)
        {
            RuleFor(x => x.UsuarioId)
                .NotEqual(Guid.Empty)
                .WithMessage("Usuário do token é obrigatório.");

            RuleFor(x => x.Token)
                .NotEmpty()
                .Length(32)
                .WithMessage("Token de redefinição inválido.");

            RuleFor(x => x.CreatedAt)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Data de criação inválida.");

            RuleFor(x => x.DataExpiracao)
                .GreaterThan(x => x.CreatedAt)
                .WithMessage("Data de expiração deve ser posterior à criação.");

            RuleFor(x => x)
                .Must(t => !t.Utilizado || t.DataUtilizacao.HasValue)
                .WithMessage("Tokens utilizados devem possuir data de utilização.");
        }

        public async Task<bool> IsValid(TokenRedefinicaoSenha token)
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
