using Onix.Writebook.Books.Domain.Entities;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Domain.Interfaces
{
    public interface IBookValidator
    {
        Task<bool> IsValid(Book book);
    }
}
