using FluentValidation;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Books.Domain.Entities;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Validators;
using Onix.Writebook.Core.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Domain.Validators
{
    public class BookValidator : BaseValidator<Book>, IBookValidator
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IStringLocalizer<TextResource> _stringLocalizer;

        public BookValidator(
            INotificationContext notificationContext,
            IUsuarioRepository usuarioRepository,
            IStringLocalizer<TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _usuarioRepository = usuarioRepository;
            _stringLocalizer = stringLocalizer;

            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(_stringLocalizer?.GetString("BookTitleRequired"));
        }

        public async Task<bool> IsValid(Book book)
        {
            if (book == null) return false;

            if (book.UsuarioId == default || !await _usuarioRepository.Exists(book.UsuarioId))
            { 
                NotificationContext.AddError(_stringLocalizer.GetString("ObjetoNaoEncontrado", book.UsuarioId));
            }

            var validationResults = Validate(book);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}
