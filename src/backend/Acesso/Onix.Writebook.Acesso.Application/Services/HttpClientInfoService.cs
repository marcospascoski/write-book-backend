using Microsoft.AspNetCore.Http;
using Onix.Writebook.Acesso.Application.Interfaces;
using System;
using System.Linq;

namespace Onix.Writebook.Acesso.Application.Services
{
    public class HttpClientInfoService(IHttpContextAccessor httpContextAccessor) : IHttpClientInfoService
    {
        public string GetClientIpAddress()
        {
            var context = httpContextAccessor?.HttpContext;
            if (context == null)
                return "0.0.0.0";

            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                    return ips[0].Trim();
            }

            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            return context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        }

        public string GetUserAgent()
        {
            var context = httpContextAccessor?.HttpContext;
            if (context == null)
                return "Unknown";

            var userAgent = context.Request.Headers.UserAgent.ToString();

            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            return userAgent[..Math.Min(userAgent.Length, 500)];
        }
    }
}
