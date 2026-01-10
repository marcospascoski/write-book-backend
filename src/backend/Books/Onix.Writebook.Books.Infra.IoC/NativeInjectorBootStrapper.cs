using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Onix.Writebook.Books.Application.Interfaces;
using Onix.Writebook.Books.Application.Services;
using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Books.Domain.Validators;
using Onix.Writebook.Books.Infra.Data.Context;
using Onix.Writebook.Books.Infra.Data.Repositories;
using Onix.Writebook.Books.Infra.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Infra.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void AddDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BooksDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }
        public static void AddConfiguration(IServiceCollection services)
        {
            // Application
            services.AddScoped<IBookAppService, BookAppService>();

            // Domain
            services.AddScoped<IBookValidator, BookValidator>();                

            // Infra Data
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IBooksUnitOfWork, BooksUnitOfWork>();
            
        }
    }
}
