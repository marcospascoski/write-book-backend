using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface ITokenBlacklistService
    {
        Task CadastrarNaBlackListAsync(string token);
        Task<bool> EstaNaBlackListAsync(string token);
    }
}
