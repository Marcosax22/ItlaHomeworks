using BarrioBook.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public List<Order> Orders { get; set; } = new();
        public List<Sale> Sales { get; set; } = new();
    }
}
