using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task Cadastrar(RefreshToken refreshToken);
        void Alterar(RefreshToken refreshToken);
        Task<RefreshToken> PesquisarPorTokenAsync(string token);
        Task RevogarTokensUsuarioAsync(Guid usuarioId);
        Task<int> ContarTokensAtivosUsuarioAsync(Guid usuarioId);
    }
}