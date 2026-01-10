using Onix.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.ValueObjects
{
    public class SaltValueObject
    {
        public string Valor { get; private set; }
        public static int SaltLengthInBytes = 16;

        public SaltValueObject(string salt)
        {
            Valor = salt;
        }
        public SaltValueObject()
        {
            Valor = EncryptionHelper.GenerateRandomSecret(SaltLengthInBytes);
        }
    }
}
