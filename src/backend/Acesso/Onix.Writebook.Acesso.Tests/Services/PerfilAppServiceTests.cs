using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;
using Onix.Writebook.Acesso.Domain.Entities;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Core.Domain.Enums;
using Onix.Writebook.Core.Resources;
using Onix.Writebook.Acesso.Tests.Moqs;
using Xunit;

namespace Onix.Writebook.Acesso.Tests.Services
{
    public class PerfilAppServiceTests
    {
        private readonly INotificationContext _notificationContext;
        private readonly IPerfilAppService _perfilAppService;
        private readonly IPerfilRepository _perfilRepository;
        private readonly IAcessosUnitOfWork _acessosUnitOfWork;
        private readonly IStringLocalizer<TextResource> _stringLocalizer;

        public PerfilAppServiceTests(
            INotificationContext notificationContext,
            IPerfilAppService perfilAppService,
            IPerfilRepository perfilRepository,
            IAcessosUnitOfWork acessosUnitOfWork,
            IStringLocalizer<TextResource> stringLocalizer)
        {
            _notificationContext = notificationContext;
            _perfilAppService = perfilAppService;
            _perfilRepository = perfilRepository;
            _acessosUnitOfWork = acessosUnitOfWork;
            _stringLocalizer = stringLocalizer;
        }

        #region Cadastrar Perfil Tests

        [Fact]
        public async Task Deve_cadastrar_perfil_com_sucesso()
        {
            // Arrange
            var perfilViewModel = new PerfilViewModel
            {
                Nome = "Administrador",
            };

            // Act
            var result = await _perfilAppService.Cadastrar(perfilViewModel);

            // Assert
            Assert.True(result > 0);
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_cadastrar_perfil_sem_nome()
        {
            // Arrange
            var perfilViewModel = new PerfilViewModel
            {
                Nome = ""
            };

            // Act
            var result = await _perfilAppService.Cadastrar(perfilViewModel);

            // Assert
            Assert.Equal(0, result);
            Assert.True(_notificationContext.HasErrors);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_cadastrar_perfil_duplicado()
        {
            // Arrange
            var perfilViewModel1 = new PerfilViewModel
            {
                Nome = "Gerente"
            };

            await _perfilAppService.Cadastrar(perfilViewModel1);
            _notificationContext.Clear();

            var perfilViewModel2 = new PerfilViewModel
            {
                Nome = "Gerente"
            };

            // Act
            var result = await _perfilAppService.Cadastrar(perfilViewModel2);

            // Assert
            Assert.Equal(0, result);
            Assert.True(_notificationContext.HasErrors);
        }

        #endregion

        #region Alterar Perfil Tests

        [Fact]
        public async Task Deve_alterar_perfil_com_sucesso()
        {
            // Arrange
            var perfilId = await CriarPerfil("Operador");
            var perfil = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            _acessosUnitOfWork.Untrack<Perfil>(perfil);

            var perfilViewModel = new PerfilViewModel
            {
                Id = perfilId,
                Nome = "Operador Senior"
            };

            // Act
            await _perfilAppService.Alterar(perfilViewModel);

            // Assert
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);

            var perfilAlterado = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            Assert.Equal("Operador Senior", perfilAlterado.Nome);
        }

        [Fact]
        public async Task Deve_retornar_erro_ao_alterar_perfil_inexistente()
        {
            // Arrange
            var perfilViewModel = new PerfilViewModel
            {
                Id = 999999,
                Nome = "Perfil Inexistente"
            };

            // Act
            await _perfilAppService.Alterar(perfilViewModel);

            // Assert
            Assert.True(_notificationContext.HasErrors);
            Assert.Contains(_notificationContext.GetErrors(), 
                e => e.Message.Contains("não encontrado") || e.Message.Contains("not found"));
        }

        #endregion

        #region Alterar Status Tests

        [Fact]
        public async Task Deve_alterar_status_perfil_para_inativo()
        {
            // Arrange
            var perfilId = await CriarPerfil("Suporte");
            var perfil = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            _acessosUnitOfWork.Untrack<Perfil>(perfil);

            var alterarStatusViewModel = new PerfilAlterarStatusViewModel<long>
            {
                Id = perfilId,
                Status = EStatusEntidade.Inativo.ToString()
            };

            // Act
            await _perfilAppService.AlterarStatus(alterarStatusViewModel);

            // Assert
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);

            var perfilAtualizado = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            Assert.Equal(EStatusEntidade.Inativo, perfilAtualizado.Status);
        }

        [Fact]
        public async Task Deve_alterar_status_perfil_para_ativo()
        {
            // Arrange
            var perfilId = await CriarPerfil("Vendedor");
            var perfil = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            _acessosUnitOfWork.Untrack<Perfil>(perfil);

            var alterarStatusViewModel = new PerfilAlterarStatusViewModel<long>
            {
                Id = perfilId,
                Status = EStatusEntidade.Ativo.ToString()
            };

            // Act
            await _perfilAppService.AlterarStatus(alterarStatusViewModel);

            // Assert
            Assert.False(_notificationContext.HasErrors);
            Assert.True(_notificationContext.Success);

            var perfilAtualizado = await _perfilRepository.PesquisarPorIdAsync(perfilId);
            Assert.Equal(EStatusEntidade.Ativo, perfilAtualizado.Status);
        }

        #endregion

        #region Pesquisar Perfil Tests

        [Fact]
        public async Task Deve_pesquisar_perfil_por_id_com_sucesso()
        {
            // Arrange
            var perfilId = await CriarPerfil("Analista");

            // Act
            var result = await _perfilAppService.PesquisarPorId(perfilId);

            // Assert
            Assert.Equal(perfilId, result.Id);
            Assert.Equal("Analista", result.Nome);
        }

        [Fact]
        public async Task Deve_retornar_null_ao_pesquisar_perfil_inexistente()
        {
            // Arrange
            var perfilId = 999999L;

            // Act
            var result = await _perfilAppService.PesquisarPorId(perfilId);

            // Assert
            // Assert.Null(result); // Removed: PerfilViewModel is a value type (struct), cannot be null
            Assert.Equal(default(PerfilViewModel), result);
        }

        #endregion

        #region Paginar Perfis Tests

        [Fact]
        public async Task Deve_paginar_perfis_com_sucesso()
        {
            // Arrange
            await CriarPerfil("Perfil 1");
            await CriarPerfil("Perfil 2");
            await CriarPerfil("Perfil 3");

            var filtro = FiltroPerfilViewModelMoq.GetFiltroPerfilViewModel();

            // Act
            var result = await _perfilAppService.Paginar(filtro);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.TotalItems >= 3);
        }

        [Fact]
        public async Task Deve_filtrar_perfis_por_texto()
        {
            // Arrange
            await CriarPerfil("Coordenador");
            await CriarPerfil("Supervisor");

            var filtro = FiltroPerfilViewModelMoq.GetFiltroPerfilViewModelComTexto("Coordenador");

            // Act
            var result = await _perfilAppService.Paginar(filtro);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.TotalItems >= 1);
            Assert.Contains(result.Items, p => p.Nome.Contains("Coordenador"));
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

        #endregion
    }
}
