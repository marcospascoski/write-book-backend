using Onix.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.ValueObjects
{
    public class SenhaValueObject
    {
        public static EEncryptionType EncryptionType => EEncryptionType.SHA256;

        private readonly string ValorBase;
        private readonly string Salt;
        public string Valor { get; private set; }

        protected SenhaValueObject()
        {
            ValorBase = null;
            Salt = null;
        }
        public SenhaValueObject(string valor)
        {
            Valor = valor;
        }
        public SenhaValueObject(string valor, string salt)
        {
            ValorBase = valor;
            Salt = salt;
            CriptografarSenhaComSalt(valor, salt);
        }

        private void CriptografarSenhaComSalt(string valor, string salt)
        {
            Valor = EncryptionHelper.Encrypt(valor + salt, EncryptionType);
        }
        public bool EstaValida()
        {
            // Senha vazia
            if (string.IsNullOrWhiteSpace(Valor))
            {
                return false;
            }
            // Senha vinda do banco de dados
            if (string.IsNullOrWhiteSpace(ValorBase))
            {
                return !string.IsNullOrWhiteSpace(Valor)
                    && Valor.Length == EncryptionHelper.CharactersCount(EncryptionType);
            }
            // Senha determinada pela interface
            return ValorBase.Length >= 3
                && ValorBase.Length <= 10
                && !ValorBase.Contains(" ");
        }

        // M�todo para validar a senha fornecida
        public bool ValidarSenha(string senhaInformada, string salt)
        {
            string senhaComSalt = senhaInformada + salt;
            string senhaCriptografada = EncryptionHelper.Encrypt(senhaComSalt, EncryptionType);
            var resultado = EncryptionHelper.SlowEquals(senhaCriptografada, Valor);
            return resultado;
        }
    }
}
