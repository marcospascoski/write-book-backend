using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Onix.Writebook.Acesso.Application.Interfaces;
using Onix.Writebook.Acesso.Application.Services;
using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Domain.Validators;
using Onix.Writebook.Acesso.Infra.Data.Context;
using Onix.Writebook.Acesso.Infra.Data.Repositories;
using Onix.Writebook.Acesso.Infra.Data.UnitOfWork;

namespace Onix.Writebook.Acesso.Infra.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void AddDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AcessosDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }
        public static void AddConfiguration(IServiceCollection services)
        {
            // Application
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IPerfilAppService, PerfilAppService>();
            services.AddScoped<IPermissaoAppService, PermissaoAppService>();
            services.AddScoped<IPerfilPermissaoAppService, PerfilPermissaoAppService>();
            services.AddScoped<IAuthAppService, AuthAppService>();
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
            
            // Domain
            services.AddScoped<IUsuarioValidator, UsuarioValidator>();
            services.AddScoped<IPerfilValidator, PerfilValidator>();
            services.AddScoped<IPermissaoValidator, PermissaoValidator>();
            services.AddScoped<IPerfilPermissaoValidator, PerfilPermissaoValidator>();
            services.AddScoped<ITokenRedefinicaoSenhaValidator, TokenRedefinicaoSenhaValidator>();


            // Infra Data
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenRedefinicaoSenhaRepository, TokenRedefinicaoSenhaRepository>();
            services.AddScoped<IAcessosUnitOfWork, AcessosUnitOfWork>();
            services.AddScoped<IPerfilRepository, PerfilRepository>();
            services.AddScoped<IPermissaoRepository, PermissaoRepository>();
            services.AddScoped<IPerfilPermissaoRepository, PerfilPermissaoRepository>();
        }
    }
}
