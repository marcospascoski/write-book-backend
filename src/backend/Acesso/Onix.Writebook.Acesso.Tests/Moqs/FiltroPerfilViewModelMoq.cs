using Onix.Framework.Tests;
using Onix.Writebook.Acesso.Application.ViewModels.Perfil;

namespace Onix.Writebook.Acesso.Tests.Moqs
{
    public static class FiltroPerfilViewModelMoq
    {
        public static FiltroPerfilViewModel GetFiltroPerfilViewModel(
            string propertyName = null,
            bool isDescending = false,
            int pageSize = 10,
            int currentPage = 0,
            string texto = null)
        {
            return new FiltroPerfilViewModel(
                propertyName: propertyName ?? "Nome",
                isDescending: isDescending,
                pageSize: pageSize,
                currentPage: currentPage,
                texto: texto ?? string.Empty);
        }

        public static FiltroPerfilViewModel GetFiltroPerfilViewModelComTexto(
            string texto,
            string propertyName = null,
            bool isDescending = false,
            int pageSize = 10,
            int currentPage = 0)
        {
            return new FiltroPerfilViewModel(
                propertyName: propertyName ?? "Nome",
                isDescending: isDescending,
                pageSize: pageSize,
                currentPage: currentPage,
                texto: texto);
        }

        public static FiltroPerfilViewModel GetFiltroPerfilViewModelOrdenado(
            string propertyName,
            bool isDescending,
            int pageSize = 10,
            int currentPage = 0,
            string texto = null)
        {
            return new FiltroPerfilViewModel(
                propertyName: propertyName,
                isDescending: isDescending,
                pageSize: pageSize,
                currentPage: currentPage,
                texto: texto ?? string.Empty);
        }

        public static FiltroPerfilViewModel GetFiltroPerfilViewModelPaginado(
            int pageSize,
            int currentPage,
            string propertyName = null,
            bool isDescending = false,
            string texto = null)
        {
            return new FiltroPerfilViewModel(
                propertyName: propertyName ?? "Nome",
                isDescending: isDescending,
                pageSize: pageSize,
                currentPage: currentPage,
                texto: texto ?? string.Empty);
        }
    }
}
