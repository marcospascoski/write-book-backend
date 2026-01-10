using Onix.Writebook.Core.Domain.Entities;
using System;

namespace Onix.Writebook.Books.Domain.Entities
{
    public class Usuario : AggregateRootEntity
    {
        public Guid Id { get; private set; }
    }
}
