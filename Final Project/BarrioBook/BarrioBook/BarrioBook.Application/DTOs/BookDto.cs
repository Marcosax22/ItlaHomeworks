using System.ComponentModel.DataAnnotations;

namespace BarrioBook.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public decimal SalePrice { get; set; }

        public int CurrentStock { get; set; }

        public int? SupplierId { get; set; }

        public string? SupplierName { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Author { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        public int? SupplierId { get; set; }

        public string? ImageUrl { get; set; }
    }

    public class UpdateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Author { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal SalePrice { get; set; }

        [Range(0, int.MaxValue)]
        public int CurrentStock { get; set; }

        public int? SupplierId { get; set; }

        public string? ImageUrl { get; set; }
    }
}
