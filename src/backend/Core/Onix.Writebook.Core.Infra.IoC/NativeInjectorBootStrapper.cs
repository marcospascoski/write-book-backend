using Microsoft.Extensions.DependencyInjection;
using Onix.Framework.Notifications.Implementation;
using Onix.Framework.Notifications.Interfaces;
using Onix.Writebook.Core.Application.Interfaces;
using Onix.Writebook.Core.Application.Services;

namespace Onix.Writebook.Core.Infra.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void AddConfiguration(IServiceCollection services)
        {
            services.AddLocalization();
            services.AddScoped<INotificationContext, NotificationContext>();
            services.AddScoped<IEmailAppService, EmailAppService>();
        }
    }
}