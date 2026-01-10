using Onix.Writebook.Books.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onix.Writebook.Books.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public virtual Usuario Usuario { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public int WordCount { get; private set; }
        public int Order { get; private set; }
        public ETypeBook Type { get; private set;  }
        public Book() { }

        public Book(
            Guid id,
            Guid usuarioId,
            string title,
            string content,
            int wordCount,
            int order)
        {
            Id = id;
            UsuarioId = usuarioId;
            Title = title;
            Content = content;
            WordCount = wordCount;
            Order = order;
        }
    }
}
