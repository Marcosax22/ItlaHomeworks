using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Application.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class CreateSaleDto
    {
        public int? CustomerId { get; set; }
        public List<CreateSaleItemDto> Items { get; set; } = new();
    }

    public class SaleItemDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateSaleItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

}
