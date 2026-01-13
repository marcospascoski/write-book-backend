using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Application.ViewModels;
using System;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class RefreshTokenViewModelMoq
    {
        private const string NotProvided = "__NOT_PROVIDED__";

        public static RefreshTokenViewModel GetRefreshTokenViewModel(
            Guid? usuarioId = null,
            string ipAddress = NotProvided,
            string userAgent = NotProvided)
        {
            return new RefreshTokenViewModel
            {
                UsuarioId = usuarioId ?? Guid.NewGuid(),
                IPAddress = ipAddress == NotProvided ? "192.168.1.1" : ipAddress,
                UserAgent = userAgent == NotProvided ? "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36" : userAgent
            };
        }

        public static RefreshTokenViewModel GetRefreshTokenViewModelComUsuarioId(Guid usuarioId)
        {
            return new RefreshTokenViewModel
            {
                UsuarioId = usuarioId,
                IPAddress = "192.168.1.100",
                UserAgent = "Mozilla/5.0 Test Browser"
            };
        }

        public static RefreshTokenViewModel GetRefreshTokenViewModelComIPCustomizado(string ipAddress)
        {
            return new RefreshTokenViewModel
            {
                UsuarioId = Guid.NewGuid(),
                IPAddress = ipAddress,
                UserAgent = "Mozilla/5.0 Test Browser"
            };
        }

        public static RefreshTokenViewModel GetRefreshTokenViewModelComUserAgentCustomizado(string userAgent)
        {
            return new RefreshTokenViewModel
            {
                UsuarioId = Guid.NewGuid(),
                IPAddress = "192.168.1.1",
                UserAgent = userAgent
            };
        }
    }
}
