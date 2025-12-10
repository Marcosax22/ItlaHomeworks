using BarrioBook.Domain.Core;

namespace BarrioBook.Domain.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public int CurrentStock { get; set; }
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
