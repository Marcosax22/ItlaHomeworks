using BarrioBook.Domain.Core;

namespace BarrioBook.Domain.Entities
{

    public class Order : BaseEntity
    {
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = OrderStatusNames.Pending;

        public List<OrderItem> Items { get; set; } = new();

        public Sale? Sale { get; set; }
    }

    public static class OrderStatusNames
    {
        public const string Pending = "Pending";    
        public const string Cancelled = "Cancelled"; 
        public const string Paid = "Paid";       
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
