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
    public class RefreshTokenRepository(AcessosDbContext context) : EFCoreRepository<RefreshToken>(context), IRefreshTokenRepository
    {
        public async Task Cadastrar(RefreshToken refreshToken)
        {
            await base.AddAsync<RefreshToken>(refreshToken);
        }

        public void Alterar(RefreshToken refreshToken)
        {
            base.Update<RefreshToken>(refreshToken);
        }

        public async Task<RefreshToken> PesquisarPorTokenAsync(string token)
        {
            return await base.FirstOrDefaultAsync<RefreshToken>(
                x => x.Token == token,
                trackChanges: false,
                include: nameof(RefreshToken.Usuario));
        }

        public async Task RevogarTokensUsuarioAsync(Guid usuarioId)
        {
            var tokens = await base.GetAsync<RefreshToken>(
                x => x.UsuarioId == usuarioId && !x.Revogado,
                trackChanges: true);

            foreach (var token in tokens)
            {
                token.Revogar();
                base.Update<RefreshToken>(token);
            }
        }

        public async Task<int> ContarTokensAtivosUsuarioAsync(Guid usuarioId)
        {
            return await base.CountAsync<RefreshToken>(
                x => x.UsuarioId == usuarioId && !x.Revogado && x.DataExpiracao > DateTime.UtcNow);
        }
    }
}