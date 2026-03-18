using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pharmacy.Domain.Entities;
using Pharmacy.Domain.Repositories.Contarct;
using Pharmacy.Services.Dtos.ShippingDtos;

namespace Pharmacy.Services
{
    public class AreaShippingFeeService : IAreaShippingFeeService
    {
        private readonly IAreaShippingFeeRepository _repository;

        public AreaShippingFeeService(IAreaShippingFeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<AreaShippingFeeToReturnDto> CreateAsync(AreaShippingFeeDto dto)
        {
            var entity = new AreaShippingFee
            {
                Area = dto.Area,
                Fee = dto.Fee,
                IsActive = dto.IsActive
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return new AreaShippingFeeToReturnDto
            {
                Id = entity.Id,
                Area = entity.Area,
                Fee = entity.Fee,
                IsActive = entity.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<AreaShippingFeeToReturnDto>> GetAllAsync()
        {
            var fees = await _repository.GetAllAsync();
            return fees.Select(f => new AreaShippingFeeToReturnDto
            {
                Id = f.Id,
                Area = f.Area,
                Fee = f.Fee,
                IsActive = f.IsActive
            }).ToList();
        }

        public async Task<AreaShippingFeeToReturnDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return new AreaShippingFeeToReturnDto
            {
                Id = entity.Id,
                Area = entity.Area,
                Fee = entity.Fee,
                IsActive = entity.IsActive
            };
        }

        public async Task<decimal> ResolveFeeByAreaAsync(string area)
        {
            var entity = await _repository.GetByAreaAsync(area);
            if (entity == null || !entity.IsActive)
                throw new System.Exception($"Active shipping fee for area '{area}' not found.");

            return entity.Fee;
        }

        public async Task<AreaShippingFeeToReturnDto?> UpdateAsync(int id, AreaShippingFeeDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Area = dto.Area;
            entity.Fee = dto.Fee;
            entity.IsActive = dto.IsActive;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return new AreaShippingFeeToReturnDto
            {
                Id = entity.Id,
                Area = entity.Area,
                Fee = entity.Fee,
                IsActive = entity.IsActive
            };
        }
    }
}
