using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrioBook.Application.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public System.DateTime RegisteredAt { get; set; }
    }

    public class CreateCustomerDto
    {
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateCustomerDto
    {
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

}
