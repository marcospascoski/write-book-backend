using Onix.Writebook.Books.Domain.Interfaces;
using Onix.Writebook.Books.Infra.Data.Context;
using Onix.Framework.Infra.Data.EFCore;

namespace Onix.Writebook.Books.Infra.Data.UnitOfWork
{
    public class BooksUnitOfWork(BooksDbContext context) : EFCoreUnitOfWork(context), IBooksUnitOfWork
    {
    }
}
