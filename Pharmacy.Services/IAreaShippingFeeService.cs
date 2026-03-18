using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy.Services.Dtos.ShippingDtos;

namespace Pharmacy.Services
{
    public interface IAreaShippingFeeService
    {
        Task<IReadOnlyList<AreaShippingFeeToReturnDto>> GetAllAsync();
        Task<AreaShippingFeeToReturnDto?> GetByIdAsync(int id);
        Task<AreaShippingFeeToReturnDto> CreateAsync(AreaShippingFeeDto dto);
        Task<AreaShippingFeeToReturnDto?> UpdateAsync(int id, AreaShippingFeeDto dto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> ResolveFeeByAreaAsync(string area);
    }
}
