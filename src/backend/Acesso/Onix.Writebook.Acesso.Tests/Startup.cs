using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Onix.Writebook.Acesso.Infra.Data.Context;
using AutoMapper;
using System; // added for Guid

namespace Onix.Writebook.Acesso.Tests
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Core registrations (Localization, NotificationContext)
            Onix.Writebook.Core.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);

            // Acesso registrations (Repositories, Services, Validators)
            Onix.Writebook.Acesso.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);

            // Register AutoMapper profiles used by the Acesso project so IMapper is available in tests
            services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

            // Use isolated InMemory database per test run to avoid cross-test contamination
            services.AddDbContext<AcessosDbContext>(options =>
                options.UseInMemoryDatabase($"AcessoTests_{Guid.NewGuid()}")
            );
            
        }
    }
}
