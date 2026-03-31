using BarrioBook.Application.DTOs;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Application.Services
{
    public class CustomerService
    {
        private readonly UnitOfWork _uow;

        public CustomerService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            return await _uow.Customers.Query()
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    Email = c.Email,
                    RegisteredAt = c.RegisteredAt
                })
                .ToListAsync();
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var c = await _uow.Customers.GetByIdAsync(id);
            if (c == null) return null;

            return new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                Email = c.Email,
                RegisteredAt = c.RegisteredAt
            };
        }

        public async Task<int> CreateAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Email = dto.Email
            };

            await _uow.Customers.AddAsync(customer);
            await _uow.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _uow.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            customer.Name = dto.Name;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;

            _uow.Customers.Update(customer);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _uow.Customers.GetByIdAsync(id);
            if (customer == null) return false;

            _uow.Customers.Remove(customer);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
