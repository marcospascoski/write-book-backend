using Onix.Writebook.Acesso.Domain.Enums;
using Onix.Writebook.Acesso.Domain.ValueObjects;
using Onix.Writebook.Core.Domain.Entities;
using System;

namespace Onix.Writebook.Acesso.Domain.Entities
{
    public class Usuario : AggregateRootEntity
    {
        public Guid Id { get; private set; }
       // public long PerfilId { get; private set; } = 1;
        //public virtual Perfil Perfil { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }        
        public SenhaValueObject Senha { get; private set; }
        public SaltValueObject Salt { get; private set; }
        public EStatusUsuario Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime Modificado { get; private set; }
        protected Usuario() { }

        public Usuario(
            Guid id, 
            string nome,
            string email,
            SenhaValueObject senha,
            SaltValueObject salt,
            EStatusUsuario statusUsuario)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Senha = senha;
            Salt = salt;
            Status = statusUsuario;
            CreatedAt = DateTime.UtcNow;
        }

        public void AlterarDados(Usuario usuarioAlterarDados)
        {
            Nome = usuarioAlterarDados.Nome;
            Email = usuarioAlterarDados.Email;
            Modificado = DateTime.UtcNow;

            if (usuarioAlterarDados.Senha != null && !string.IsNullOrWhiteSpace(usuarioAlterarDados.Senha.Valor))
            {
                Salt = new SaltValueObject();
                Senha = new SenhaValueObject(usuarioAlterarDados.Senha.Valor, Salt.Valor);
            }
        }

        public void AlterarStatus(EStatusUsuario status)
        {
            this.Status = status;
        }

        public static class Factory
        {
            public static Usuario Create(Usuario prototype)
            {
                var salt = new SaltValueObject();
                var senhaCriptografada = new SenhaValueObject(prototype.Senha.Valor, salt.Valor);
                
                return new Usuario
                {

                    Id = Guid.NewGuid(),
                    Nome = prototype.Nome,
                    //Perfil = prototype.Perfil,
                    Email = prototype.Email,
                    Senha = senhaCriptografada,
                    Salt = salt,
                    Status = EStatusUsuario.PendenteConfirmacao,
                };
            }
        }
    }
}
