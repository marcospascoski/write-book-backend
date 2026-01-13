using Onix.Writebook.Sistema.Domain.Interfaces;
using Onix.Writebook.Sistema.Infra.Data.Context;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Sistema.Infra.Data.UnitOfWork
{
    public class SistemaUnitOfWork(SistemaDbContext context) : EFCoreUnitOfWork(context), ISistemaUnitOfWork
    {
        
    }
}
