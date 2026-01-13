using Onix.Writebook.Acesso.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly HashSet<string> _blacklistedTokens = new();

        public async Task CadastrarNaBlackListAsync(string token)
        {
            _blacklistedTokens.Add(token);
            await Task.CompletedTask;
        }

        public async Task<bool> EstaNaBlackListAsync(string token)
        {
            return await Task.FromResult(_blacklistedTokens.Contains(token));
        }
    }
}
