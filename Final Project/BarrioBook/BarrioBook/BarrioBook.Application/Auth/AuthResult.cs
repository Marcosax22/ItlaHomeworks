using BarrioBook.Application.DTOs;

namespace BarrioBook.Application.Auth
{
    public class AuthResult
    {
        public string Token { get; set; } = null!;
        public CustomerDto Customer { get; set; } = null!;
    }
}
