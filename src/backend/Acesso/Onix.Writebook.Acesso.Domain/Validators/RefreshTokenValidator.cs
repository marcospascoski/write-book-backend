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
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IStringLocalizer<TextResource> _stringLocalizer;

        public RefreshTokenValidator(
            INotificationContext notificationContext,
            IRefreshTokenRepository refreshTokenRepository,
            IStringLocalizer<TextResource> stringLocalizer)
            : base(notificationContext)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _stringLocalizer = stringLocalizer;

            // Validação de UsuarioId
            RuleFor(x => x.UsuarioId)
                .NotEqual(Guid.Empty)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenUsuarioObrigatorio"));

            // Validação de Token
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage(_stringLocalizer?.GetString("ErroTokenFormatoInvalido"))
                .MaximumLength(64)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenFormatoInvalido"));

            // Validação de IPAddress
            RuleFor(x => x.IPAddress)
                .NotEmpty()
                .WithMessage(_stringLocalizer?.GetString("ErroTokenIPAddressObrigatorio"))
                .MaximumLength(45)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenIPAddressTamanhoInvalido"));

            // Validação de UserAgent
            RuleFor(x => x.UserAgent)
                .NotEmpty()
                .WithMessage(_stringLocalizer?.GetString("ErroTokenUserAgentObrigatorio"))
                .MaximumLength(500)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenUserAgentTamanhoInvalido"));

            // Validação de CreatedAt
            RuleFor(x => x.CreatedAt)
                .GreaterThan(DateTime.MinValue)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataCriacao"))
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataCriacaoFuturo"));

            // Validação de DataExpiracao
            RuleFor(x => x.DataExpiracao)
                .GreaterThan(x => x.CreatedAt)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataExpiracao"))
                .LessThanOrEqualTo(x => x.CreatedAt.AddDays(365))
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataExpiracaoMaxima"));

            // Validação de Token Revogado
            RuleFor(x => x)
                .Must(t => !t.Revogado || t.DataRevogacao.HasValue)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenRevogadoSemData"));

            // Validação adicional: Data de revogação não pode ser anterior à criação
            RuleFor(x => x)
                .Must(t => !t.DataRevogacao.HasValue || t.DataRevogacao.Value >= t.CreatedAt)
                .WithMessage(_stringLocalizer?.GetString("ErroTokenDataRevogacaoAnteriorCriacao"));
        }

        public async Task<bool> IsValid(RefreshToken token)
        {
            if (token == null)
            {
                NotificationContext.AddError(_stringLocalizer?.GetString("ErroTokenNulo"));
                return false;
            }
            
            var jaCadastrado = await _refreshTokenRepository.JaCadastrado(token.Id, token.UsuarioId);

            if (jaCadastrado)
            {
                    NotificationContext.AddError(_stringLocalizer?.GetString("ErroTokenJaExiste"));
            }

            // Validação assíncrona: Verifica limite de tokens ativos por usuário (máximo 5)
            if (token.UsuarioId != Guid.Empty && !token.Revogado)
            {
                var tokensAtivos = await _refreshTokenRepository.ContarTokensAtivosUsuarioAsync(token.UsuarioId);

                // Se for um token novo (não existe ainda no banco) e já atingiu o limite
                if (!jaCadastrado)
                {
                    if (tokensAtivos >= 5)
                    {
                        NotificationContext.AddError(_stringLocalizer?.GetString("ErroLimiteTokensAtivos"));
                    }
                }
            }

            var validationResults = Validate(token);
            ProcessValidationResults(validationResults);
            return !NotificationContext.HasErrors;
        }
    }
}