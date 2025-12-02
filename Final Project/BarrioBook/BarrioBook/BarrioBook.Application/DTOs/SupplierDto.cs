using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Application.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ContactInfo { get; set; }
        public System.DateTime AssociatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = null!;
        public string? ContactInfo { get; set; }
    }

    public class UpdateSupplierDto
    {
        public string Name { get; set; } = null!;
        public string? ContactInfo { get; set; }
    }
}
