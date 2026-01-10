using Microsoft.Extensions.DependencyInjection;

namespace Onix.Writebook.WebApi.Config
{
    public static class DbContextConfig
    {
        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, string connectionString)
        {
            Sistema.Infra.IoC.NativeInjectorBootStrapper.AddDbContext(services, connectionString);
            Acesso.Infra.IoC.NativeInjectorBootStrapper.AddDbContext(services, connectionString);

            return services;
        }
    }
}
