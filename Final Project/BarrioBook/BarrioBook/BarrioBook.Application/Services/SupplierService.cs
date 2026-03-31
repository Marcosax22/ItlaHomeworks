using BarrioBook.Application.DTOs;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Application.Services
{
    public class SupplierService
    {
        private readonly UnitOfWork _uow;

        public SupplierService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<SupplierDto>> GetAllAsync()
        {
            return await _uow.Suppliers.Query()
                .Select(s => new SupplierDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ContactInfo = s.ContactInfo,
                    AssociatedAt = s.AssociatedAt
                })
                .ToListAsync();
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var s = await _uow.Suppliers.GetByIdAsync(id);
            if (s == null) return null;

            return new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactInfo = s.ContactInfo,
                AssociatedAt = s.AssociatedAt
            };
        }

        public async Task<int> CreateAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactInfo = dto.ContactInfo
            };

            await _uow.Suppliers.AddAsync(supplier);
            await _uow.SaveChangesAsync();
            return supplier.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier == null) return false;

            supplier.Name = dto.Name;
            supplier.ContactInfo = dto.ContactInfo;

            _uow.Suppliers.Update(supplier);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var supplier = await _uow.Suppliers.GetByIdAsync(id);
            if (supplier == null) return false;

            _uow.Suppliers.Remove(supplier);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
