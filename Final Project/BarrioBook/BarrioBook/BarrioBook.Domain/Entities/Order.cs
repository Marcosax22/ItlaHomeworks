using BarrioBook.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Domain.Entities
{
    public class Order : BaseEntity
    {
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // e.g.: "Reserved", "Paid", "Delivered", "Cancelled"
        public string Status { get; set; } = "Reserved";

        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
