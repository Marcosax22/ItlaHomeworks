using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        public int? CustomerId { get; set; }
        public string Status { get; set; } = "Reserved";
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
