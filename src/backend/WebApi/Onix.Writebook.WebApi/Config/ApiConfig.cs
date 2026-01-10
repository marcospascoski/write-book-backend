using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Onix.Framework.Security.JwtConfig;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace Onix.Writebook.WebApi.Config
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            // Dependencies Config
            Core.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);
            Sistema.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);

            // New Modules
            Acesso.Infra.IoC.NativeInjectorBootStrapper.AddConfiguration(services);


            // Controllers
            services
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

            // Cors
            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            // Authentication
            var authorizationBuilder = services.AddAuthorizationBuilder();
            authorizationBuilder.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            authorizationBuilder.AddPolicy("ClienteOnly", policy => policy.RequireRole("Cliente"));
            // Add other policies as needed

            // Adicionar configuração de autenticação JWT
            services.AddJwtConfiguration();
            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
