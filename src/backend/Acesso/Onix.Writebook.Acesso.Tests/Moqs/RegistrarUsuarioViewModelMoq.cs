using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Application.ViewModels.Registrar;
using Onix.Writebook.Acesso.Domain.Enums;
using System;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class RegistrarUsuarioViewModelMoq
    {
        public static RegistrarUsuarioViewModel GetUsuarioViewModel(
            string nome = null,
            string email = null,
            string senha = null)
        {
            return new RegistrarUsuarioViewModel
            {
                Nome = nome ?? ValuesMoq.CreateString(20),
                Email = email ?? $"{ValuesMoq.CreateString(10)}@test.com",
                Senha = senha ?? "Senha@123"
            };
        }
    }
}
