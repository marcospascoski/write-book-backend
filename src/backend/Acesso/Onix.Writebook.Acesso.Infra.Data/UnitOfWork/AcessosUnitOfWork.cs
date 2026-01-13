using Onix.Writebook.Acesso.Domain.Interfaces;
using Onix.Writebook.Acesso.Infra.Data.Context;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Acesso.Infra.Data.UnitOfWork
{
    public class AcessosUnitOfWork(AcessosDbContext context) : EFCoreUnitOfWork(context), IAcessosUnitOfWork
    {
        
    }
}
