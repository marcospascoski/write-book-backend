using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Domain.Entities;
using System;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class RefreshTokenMoq
    {
        public static RefreshToken GetRefreshToken(
            Guid? usuarioId = null,
            string ipAddress = null,
            string userAgent = null,
            int diasExpiracao = 30,
            bool revogado = false)
        {
            var token = new RefreshToken(
                usuarioId ?? Guid.NewGuid(),
                ipAddress ?? "192.168.1.1",
                userAgent ?? "Mozilla/5.0 Test",
                diasExpiracao
            );

            if (revogado)
            {
                token.Revogar();
            }

            return token;
        }

        public static RefreshToken GetRefreshTokenExpirado(
            Guid? usuarioId = null,
            string ipAddress = null,
            string userAgent = null)
        {
            return new RefreshToken(
                usuarioId ?? Guid.NewGuid(),
                ipAddress ?? "192.168.1.1",
                userAgent ?? "Mozilla/5.0 Test",
                diasExpiracao: -1 // Token já expirado
            );
        }

        public static RefreshToken GetRefreshTokenRevogado(
            Guid? usuarioId = null,
            string ipAddress = null,
            string userAgent = null)
        {
            var token = new RefreshToken(
                usuarioId ?? Guid.NewGuid(),
                ipAddress ?? "192.168.1.1",
                userAgent ?? "Mozilla/5.0 Test"
            );

            token.Revogar();
            return token;
        }

        public static RefreshToken GetRefreshTokenValido(
            Guid? usuarioId = null,
            string ipAddress = null,
            string userAgent = null)
        {
            return new RefreshToken(
                usuarioId ?? Guid.NewGuid(),
                ipAddress ?? "192.168.1.1",
                userAgent ?? "Mozilla/5.0 Test",
                diasExpiracao: 30
            );
        }
    }
}
