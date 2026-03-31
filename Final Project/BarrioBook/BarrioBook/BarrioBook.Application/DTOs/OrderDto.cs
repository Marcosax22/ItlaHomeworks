using System.ComponentModel.DataAnnotations;

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
        [Required]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        public int? CustomerId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class UpdateOrderDto
    {
        public int? CustomerId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
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