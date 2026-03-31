using BarrioBook.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? ContactInfo { get; set; }

        public DateTime AssociatedAt { get; set; } = DateTime.Now;

        public List<Book> SuppliedBooks { get; set; } = new();
    }
}
