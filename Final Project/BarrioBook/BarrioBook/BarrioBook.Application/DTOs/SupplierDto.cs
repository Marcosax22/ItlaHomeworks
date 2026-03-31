using System.ComponentModel.DataAnnotations;

namespace BarrioBook.Application.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ContactInfo { get; set; }

        public DateTime AssociatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public string? ContactInfo { get; set; }
    }

    public class UpdateSupplierDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public string? ContactInfo { get; set; }
    }
}
