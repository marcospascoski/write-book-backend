using Onix.Writebook.Sistema.Domain.Entities;
using Onix.Writebook.Sistema.Domain.Interfaces;
using Onix.Writebook.Sistema.Infra.Data.Context;
using Onix.Framework.Infra.Data.EFCore;
using System.Threading.Tasks;

namespace Onix.Writebook.Sistema.Infra.Data.Repositories
{
    public class ExceptionLogRepository(SistemaDbContext context) 
        : EFCoreRepository<ExceptionLog>(context), IExceptionLogRepository
    {
        public void Add(ExceptionLog exceptionLog)
        {
            base.Add<ExceptionLog>(exceptionLog);
        }
        public async Task AddAsync(ExceptionLog exceptionLog)
        {
            await base.AddAsync<ExceptionLog>(exceptionLog);
        }

        public async Task<ExceptionLog> ByMessage(string message)
        {
            return await base.FirstOrDefaultAsync<ExceptionLog>(x => x.Message == message);
        }
    }
}
