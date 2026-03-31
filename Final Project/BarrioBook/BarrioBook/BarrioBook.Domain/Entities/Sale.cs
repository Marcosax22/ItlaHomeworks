using BarrioBook.Domain.Core;

namespace BarrioBook.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }

        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }

        public List<SaleItem> Items { get; set; } = new();
    }

    public class SaleItem : BaseEntity
    {
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
