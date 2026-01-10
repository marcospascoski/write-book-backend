using FluentValidation;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using System.Threading.Tasks;
using System;

namespace Onix.Writebook.Acesso.Domain.Validators
{
    public class UsuarioValidator : BaseValidator<Usuario>, IUsuarioValidator
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioValidator(
            INotificationContext notificationContext,
            IUsuarioRepository usuarioRepository)
            : base(notificationContext)
        {
            _usuarioRepository = usuarioRepository;

            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Nome)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(128)
                .WithMessage("O nome deve ter entre 2 e 128 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256)
                .WithMessage("Email inválido.");
        }

        public async Task<bool> IsValid(Usuario usuario)
        {
            if (usuario == null) return false;

            if (await _usuarioRepository.JaCadastrado(usuario.Email, usuario.Id))
            {
                NotificationContext.AddError("ErroUsuarioJaCadatradoEmail");
            }

            var validationResults = Validate(usuario);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
