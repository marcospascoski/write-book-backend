using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Enums;
using Onix.Writebook.Acesso.Domain.ValueObjects;
using Onix.Writebook.Core.Domain.Enums;
using System;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class UsuarioMoq
    {
        private const string NotProvided = "__NOT_PROVIDED__";

        public static Usuario GetUsuario(
            Guid? id = null,
            string nome = NotProvided,
            string email = NotProvided,
            string senha = NotProvided,
            EStatusUsuario status = EStatusUsuario.PendenteConfirmacao
            )
        {
            var salt = new SaltValueObject();
            var senhaRaw = senha == NotProvided ? "Senha@123" : senha;
            var senhaValueObject = new SenhaValueObject(senhaRaw, salt.Valor);

            var nomeFinal = nome == NotProvided ? ValuesMoq.CreateString(20) : nome;
            var emailFinal = email == NotProvided ? $"{ValuesMoq.CreateString(10)}@test.com" : email;

            return new Usuario(
                id ?? Guid.NewGuid(),
                nomeFinal,
                emailFinal,
                senhaValueObject,
                salt,
                status
            );
        }

        public static Usuario GetUsuarioComEmailSpecifico(
            string email,
            string nome = null,
            string senha = null)
        {
            var salt = new SaltValueObject();
            var senhaValueObject = new SenhaValueObject(senha ?? "Senha@123", salt.Valor);

            return new Usuario(
                Guid.NewGuid(),
                nome ?? ValuesMoq.CreateString(20),
                email ?? $"{ValuesMoq.CreateString(10)}@test.com",
                senhaValueObject,
                salt,
                EStatusUsuario.PendenteConfirmacao
            );
        }

        public static Usuario GetUsuarioAtivo(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuario(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Ativo
            );
        }

        public static Usuario GetUsuarioBloqueado(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuario(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Bloqueado
            );
        }

        public static Usuario GetUsuarioInativo(
            Guid? id = null,
            string nome = null,
            string email = null)
        {
            return GetUsuario(
                id: id,
                nome: nome,
                email: email,
                status: EStatusUsuario.Inativo
            );
        }
    }
}
