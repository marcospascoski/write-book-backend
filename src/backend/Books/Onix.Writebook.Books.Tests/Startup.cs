using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using AutoMapper;
using Onix.Writebook.Books.Infra.Data.Context;
using Onix.Writebook.Books.Application.AutoMapper;
using System;

namespace Onix.Writebook.Books.Tests
{
    /// <summary>
    /// Default startup for Xunit.DependencyInjection.
    /// This enables constructor injection in test classes.
    /// </summary>
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Core registrations (Localization, NotificationContext, etc.)
            Onix.Writebook.Core.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);

            // Books registrations (Repositories, Services, Validators)
            Onix.Writebook.Books.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);

            // Isolated InMemory database per test run to avoid cross-test contamination
            services.AddDbContext<BooksDbContext>(options =>
                options.UseInMemoryDatabase($"BooksTests_{Guid.NewGuid()}")
            );

            // AutoMapper profiles
            services.AddAutoMapper(
                typeof(DomainToViewModelMappingProfile).Assembly,
                typeof(ViewModelToDomainMappingProfile).Assembly
            );
        }
    }
}
