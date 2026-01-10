using Onix.Writebook.Sistema.Domain.Entities;
using Onix.Framework.Infra.Data.Interfaces;
using System.Threading.Tasks;

namespace Onix.Writebook.Sistema.Domain.Interfaces
{
    public interface IExceptionLogRepository : IRepository<ExceptionLog>
    {
        void Add(ExceptionLog exceptionLog);
        Task AddAsync(ExceptionLog exceptionLog);
        Task<ExceptionLog> ByMessage(string message);
    }
}
