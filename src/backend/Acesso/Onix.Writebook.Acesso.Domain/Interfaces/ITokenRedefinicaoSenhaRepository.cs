using Onix.Framework.Infra.Data.Interfaces;
using Onix.Writebook.Acesso.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Onix.Writebook.Acesso.Domain.Interfaces
{
    public interface ITokenRedefinicaoSenhaRepository : IRepository<TokenRedefinicaoSenha>
    {
        Task Cadastrar(TokenRedefinicaoSenha token);
        void Alterar(TokenRedefinicaoSenha token);
        Task<TokenRedefinicaoSenha> PesquisarPorTokenAsync(string token);
        Task<TokenRedefinicaoSenha> PesquisarTokenValidoPorUsuarioAsync(Guid usuarioId);
        Task InvalidarTokensAntigos(Guid usuarioId);
    }
}
