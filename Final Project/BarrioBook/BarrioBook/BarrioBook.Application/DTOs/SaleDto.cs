using System.ComponentModel.DataAnnotations;

namespace BarrioBook.Application.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? OrderId { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class CreateSaleDto
    {
        public int? CustomerId { get; set; }

        public int? OrderId { get; set; }

        [MinLength(1, ErrorMessage = "A sale must contain at least one item.")]
        public List<CreateSaleItemDto>? Items { get; set; }
    }

    public class UpdateSaleDto
    {
        public int? CustomerId { get; set; }

        public DateTime? SaleDate { get; set; }

        [MinLength(1, ErrorMessage = "A sale must contain at least one item.")]
        public List<UpdateSaleItemDto>? Items { get; set; }
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
        [Required]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class UpdateSaleItemDto
    {
        [Required]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}