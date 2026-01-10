using Onix.Writebook.Sistema.Domain.Interfaces;
using Onix.Writebook.Sistema.Infra.Data.Context;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Sistema.Infra.Data.UnitOfWork
{
    public class SistemaUnitOfWork : EFCoreUnitOfWork, ISistemaUnitOfWork
    {
        public SistemaUnitOfWork(SistemaDbContext context) : base(context)
        {
        }
    }
}
