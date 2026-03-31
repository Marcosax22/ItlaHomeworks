using BarrioBook.Domain.Core;

namespace BarrioBook.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";

        public DateTime RegisteredAt { get; set; }

        public List<Order> Orders { get; set; } = new();
        public List<Sale> Sales { get; set; } = new();

        public Customer()
        {
            RegisteredAt = DateTime.Now;
        }

        public Customer(string name, string email, string passwordHash, string? phone = null, string role = "Customer")
        {
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Phone = phone;
            Role = role;
            RegisteredAt = DateTime.Now;
        }
    }
}
