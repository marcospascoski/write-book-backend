using Microsoft.EntityFrameworkCore;
using Onix.Framework.Infra.Data.EFCore;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Infra.Data.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Infra.Data.Repositories
{
    public class TokenRedefinicaoSenhaRepository(AcessosDbContext context) 
        : EFCoreRepository<TokenRedefinicaoSenha>(context), ITokenRedefinicaoSenhaRepository
    {
        public async Task Cadastrar(TokenRedefinicaoSenha token)
        {
            await base.AddAsync<TokenRedefinicaoSenha>(token);
        }

        public void Alterar(TokenRedefinicaoSenha token)
        {
            base.Update<TokenRedefinicaoSenha>(token);
        }

        public async Task<TokenRedefinicaoSenha> PesquisarPorTokenAsync(string token)
        {
            return await base.FirstOrDefaultAsync<TokenRedefinicaoSenha>(x => x.Token == token);
        }

        public async Task<TokenRedefinicaoSenha> PesquisarTokenValidoPorUsuarioAsync(Guid usuarioId)
        {
            return await base.Queryable<TokenRedefinicaoSenha>()
                .Where(x => x.UsuarioId == usuarioId && !x.Utilizado && x.DataExpiracao > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task InvalidarTokensAntigos(Guid usuarioId)
        {
            var tokensAtivos = await base.Queryable<TokenRedefinicaoSenha>()
                .Where(x => x.UsuarioId == usuarioId && !x.Utilizado)
                .ToListAsync();

            foreach (var token in tokensAtivos)
            {
                token.MarcarComoUtilizado();
                base.Update<TokenRedefinicaoSenha>(token);
            }
        }
    }
}
