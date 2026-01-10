using Onix.Writebook.Core.Domain.Entities;
using System;

namespace Onix.Writebook.Books.Domain.Entities
{
    public class Usuario : AggregateRootEntity
    {
        public Guid Id { get; private set; }
        
        // Parameterless constructor for EF Core
        protected Usuario() { }
        
        // Constructor for tests and business logic
        public Usuario(Guid id)
        {
            Id = id;
        }
    }
}
