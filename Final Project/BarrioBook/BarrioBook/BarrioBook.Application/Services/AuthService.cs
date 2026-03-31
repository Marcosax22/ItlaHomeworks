using BarrioBook.Application.DTOs;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarrioBook.Application.Auth
{
    public class AuthService
    {
        private readonly UnitOfWork _uow;
        private readonly JwtOptions _jwtOptions;

        public AuthService(UnitOfWork uow, IOptions<JwtOptions> jwtOptions)
        {
            _uow = uow;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<CustomerDto> RegisterAsync(RegisterCustomerDto dto)
        {
            var existing = await _uow.Customers.Query()
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (existing != null)
            {
                throw new InvalidOperationException("Email is already registered.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var customer = new Customer(dto.Name, dto.Email, passwordHash, dto.Phone);

            await _uow.Customers.AddAsync(customer);
            await _uow.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                RegisteredAt = customer.RegisteredAt
            };
        }

        public async Task<AuthResult> LoginAsync(LoginCustomerDto dto)
        {
            var customer = await _uow.Customers.Query()
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (customer == null)
            {
                throw new InvalidOperationException("Invalid credentials.");
            }

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash);
            if (!valid)
            {
                throw new InvalidOperationException("Invalid credentials.");
            }

            var token = GenerateToken(customer);

            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email,
                RegisteredAt = customer.RegisteredAt
            };

            return new AuthResult
            {
                Token = token,
                Customer = customerDto
            };
        }

        private string GenerateToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, customer.Name),
                new Claim(ClaimTypes.Role, customer.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

