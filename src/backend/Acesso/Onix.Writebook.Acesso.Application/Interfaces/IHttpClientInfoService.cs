namespace Onix.Writebook.Acesso.Application.Interfaces
{
    public interface IHttpClientInfoService
    {
        string GetClientIpAddress();
        string GetUserAgent();
    }
}
