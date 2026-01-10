using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using Onix.Writebook.Acesso.Application.ViewModels.Permissao;
using Onix.Writebook.Acesso.Application.ViewModels.PerfilPermissao;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Resources;
using Xunit;
using System.Linq;

namespace Onix.Writebook.Acesso.Tests.Services
{
    public class PerfilPermissaoAppServiceTests
    {
        private readonly INotificationContext _notificationContext;
        private readonly IPerfilPermissaoAppService _perfilPermissaoAppService;
        private readonly IPerfilPermissaoRepository _perfilPermissaoRepository;
        private readonly IPerfilAppService _perfilAppService;
        private readonly IPermissaoAppService _permissaoAppService;
        private readonly IPerfilRepository _perfilRepository;
        private readonly IPermissaoRepository _permissaoRepository;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork;
        private readonly IStringLocalizer<TextResource> _stringLocalizer;

        public PerfilPermissaoAppServiceTests(
            INotificationContext notificationContext,
            IPerfilPermissaoAppService perfilPermissaoAppService,
            IPerfilPermissaoRepository perfilPermissaoRepository,
            IPerfilAppService perfilAppService,
            IPermissaoAppService permissaoAppService,
            IPerfilRepository perfilRepository,
            IPermissaoRepository permissaoRepository,
            IAcessosUnitOfWork acessosUnitOfWork,
            IStringLocalizer<TextResource> stringLocalizer)
        {
            _notificationContext = notificationContext;
            _perfilPermissaoAppService = perfilPermissaoAppService;
            _perfilPermissaoRepository = perfilPermissaoRepository;
            _perfilAppService = perfilAppService;
            _permissaoAppService = permissaoAppService;
            _perfilRepository = perfilRepository;
            _permissaoRepository = permissaoRepository;
            _acessosUnitOfWork = acessosUnitOfWork;
            _stringLocalizer = stringLocalizer;
        }

        #region Cadastrar PerfilPermissao Tests

        [Fact]
        public async Task Deve_cadastrar_perfil_permissao_com_sucesso()
        {
            // Arrange
            var perfilId = await CriarPerfil("Administrador Sistema");
            var permissaoId = await CriarPermissao("Criar Usuario");

            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                PerfilId = perfilId,
                PermissaoId = permissaoId
            };

            // Act
            var result = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel);

            // Assert
            Assert.True(result > 0);
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_cadastrar_perfil_permissao_duplicada()
        {
            // Arrange
            var perfilId = await CriarPerfil("Gerente Vendas");
            var permissaoId = await CriarPermissao("Editar Vendas");

            // Cadastrar primeira vez
            var perfilPermissaoViewModel1 = new PerfilPermissaoViewModel
            {
                PerfilId = perfilId,
                PermissaoId = permissaoId
            };
            await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel1);
            _notificationContext.Clear();

            // Tentar cadastrar duplicado
            var perfilPermissaoViewModel2 = new PerfilPermissaoViewModel
            {
                PerfilId = perfilId,
                PermissaoId = permissaoId
            };

            // Act
            var result = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel2);

            // Assert
            Assert.Equal(0, result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_cadastrar_com_perfil_inexistente()
        {
            // Arrange
            var permissaoId = await CriarPermissao("Deletar Usuario");

            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                PerfilId = 999999, // ID inexistente
                PermissaoId = permissaoId
            };

            // Act
            var result = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel);

            // Assert
            Assert.Equal(0, result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_cadastrar_com_permissao_inexistente()
        {
            // Arrange
            var perfilId = await CriarPerfil("Supervisor");

            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                PerfilId = perfilId,
                PermissaoId = 999999 // ID inexistente
            };

            // Act
            var result = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel);

            // Assert
            Assert.Equal(0, result);
            Assert.True(_notificationContext.HasErrors);
        }

        #endregion

        #region Remover PerfilPermissao Tests

        [Fact]
        public async Task Deve_remover_perfil_permissao_com_sucesso()
        {
            // Arrange
            var perfilId = await CriarPerfil("Analista Financeiro");
            var permissaoId = await CriarPermissao("Ver Relatórios");
            var perfilPermissaoId = await CriarPerfilPermissao(perfilId, permissaoId);

            var perfilPermissao = await _perfilPermissaoRepository.PesquisarPorIdAsync(perfilPermissaoId);
            _acessosUnitOfWork.Untrack<PerfilPermissao>(perfilPermissao);

            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                Id = perfilPermissaoId
            };

            // Act
            await _perfilPermissaoAppService.Remover(perfilPermissaoViewModel);

            // Assert
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);

            var perfilPermissaoRemovido = await _perfilPermissaoRepository.PesquisarPorIdAsync(perfilPermissaoId);
            Assert.Null(perfilPermissaoRemovido);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_remover_perfil_permissao_inexistente()
        {
            // Arrange
            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                Id = 999999
            };

            // Act
            await _perfilPermissaoAppService.Remover(perfilPermissaoViewModel);

            // Assert
            Assert.True(_notificationContext.HasErrors);

            // Fix: Check error messages in the notification context for the expected text
            var errorMessages = _notificationContext.GetErrors().Select(e => e.Message);
            Assert.Contains(errorMessages, 
                e => e.Contains("não encontrado") || e.Contains("not found"));
        }

        #endregion

        #region Pesquisar PerfilPermissao Tests

        [Fact]
        public async Task Deve_pesquisar_perfil_permissao_por_id_com_sucesso()
        {
            // Arrange
            var perfilId = await CriarPerfil("Operador Sistema");
            var permissaoId = await CriarPermissao("Exportar Dados");
            var perfilPermissaoId = await CriarPerfilPermissao(perfilId, permissaoId);

            // Act
            var result = await _perfilPermissaoAppService.PesquisarPorId(perfilPermissaoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(perfilPermissaoId, result.Id);
            Assert.Equal(perfilId, result.PerfilId);
            Assert.Equal(permissaoId, result.PermissaoId);
        }

        [Fact]
        public async Task Deve_retornar_null_ao_pesquisar_perfil_permissao_inexistente()
        {
            // Arrange
            var perfilPermissaoId = 999999L;

            // Act
            var result = await _perfilPermissaoAppService.PesquisarPorId(perfilPermissaoId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Testes de Permissões Múltiplas

        [Fact]
        public async Task Deve_cadastrar_multiplas_permissoes_para_um_perfil()
        {
            // Arrange
            var perfilId = await CriarPerfil("Desenvolvedor");
            var permissao1Id = await CriarPermissao("Deploy Aplicacao");
            var permissao2Id = await CriarPermissao("Ver Logs");
            var permissao3Id = await CriarPermissao("Editar Configuracao");

            // Act
            var result1 = await CriarPerfilPermissao(perfilId, permissao1Id);
            var result2 = await CriarPerfilPermissao(perfilId, permissao2Id);
            var result3 = await CriarPerfilPermissao(perfilId, permissao3Id);

            // Assert
            Assert.True(result1 > 0);
            Assert.True(result2 > 0);
            Assert.True(result3 > 0);
            Assert.False(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_permitir_mesma_permissao_em_perfis_diferentes()
        {
            // Arrange
            var perfil1Id = await CriarPerfil("Gerente Regional");
            var perfil2Id = await CriarPerfil("Gerente Filial");
            var permissaoId = await CriarPermissao("Aprovar Vendas");

            // Act
            var result1 = await CriarPerfilPermissao(perfil1Id, permissaoId);
            var result2 = await CriarPerfilPermissao(perfil2Id, permissaoId);

            // Assert
            Assert.True(result1 > 0);
            Assert.True(result2 > 0);
            Assert.False(_notificationContext.HasErrors);
        }

        #endregion

        #region Helper Methods

        private async Task<long> CriarPerfil(string nome)
        {
            var perfilViewModel = new PerfilViewModel
            {
                Nome = nome
            };

            var perfilId = await _perfilAppService.Cadastrar(perfilViewModel);
            _notificationContext.Clear();

            return perfilId;
        }

        private async Task<long> CriarPermissao(string nome)
        {
            var permissaoViewModel = new PermissaoViewModel
            {
                Nome = nome
            };

            var permissaoId = await _permissaoAppService.Cadastrar(permissaoViewModel);
            _notificationContext.Clear();

            return permissaoId;
        }

        private async Task<long> CriarPerfilPermissao(long perfilId, long permissaoId)
        {
            var perfilPermissaoViewModel = new PerfilPermissaoViewModel
            {
                PerfilId = perfilId,
                PermissaoId = permissaoId
            };

            var perfilPermissaoId = await _perfilPermissaoAppService.Cadastrar(perfilPermissaoViewModel);
            _notificationContext.Clear();

            return perfilPermissaoId;
        }

        #endregion
    }
}
