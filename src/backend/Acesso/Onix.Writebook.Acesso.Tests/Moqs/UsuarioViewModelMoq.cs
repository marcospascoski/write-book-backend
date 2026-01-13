using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Application.ViewModels;
using Onix.Writebook.Acesso.Domain.Enums;
using System;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class UsuarioViewModelMoq
    {
        public static UsuarioViewModel GetUsuarioViewModel(
            Guid? id = null,
            string nome = null,
            string email = null,
            string senha = null,
            EStatusUsuario status = EStatusUsuario.PendenteConfirmacao)
        {
            return new UsuarioViewModel
            {
                Id = id ?? Guid.NewGuid(),
                Nome = nome ?? ValuesMoq.CreateString(20),
                Email = email ?? $"{ValuesMoq.CreateString(10)}@test.com",
                Senha = senha ?? "Senha@123",
                Status = status.ToString(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static UsuarioViewModel GetUsuarioViewModelComEmailSpecifico(
            string email,
            string nome = null,
            string senha = null)
        {
            return new UsuarioViewModel
            {
                Id = Guid.NewGuid(),
                Nome = nome ?? ValuesMoq.CreateString(20),
                Email = email,
                Senha = senha ?? "Senha@123",
                Status = EStatusUsuario.PendenteConfirmacao.ToString(),
                CreatedAt = DateTime.UtcNow
            };
        }

        public static UsuarioViewModel GetUsuarioViewModelAtivo(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuarioViewModel(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Ativo
            );
        }

        public static UsuarioViewModel GetUsuarioViewModelBloqueado(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuarioViewModel(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Bloqueado
            );
        }

        public static UsuarioViewModel GetUsuarioViewModelInativo(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuarioViewModel(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Inativo
            );
        }
    }
}
