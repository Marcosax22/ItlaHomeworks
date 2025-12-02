using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class CreateBookDto
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public int CurrentStock { get; set; }
        public int? SupplierId { get; set; }
    }

    public class UpdateBookDto
    {
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public decimal SalePrice { get; set; }
        public int CurrentStock { get; set; }
        public int? SupplierId { get; set; }
    }
}
