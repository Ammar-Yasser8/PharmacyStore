using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy.Domain.Entities;

namespace Pharmacy.Domain.Repositories.Contarct
{
    public interface IAreaShippingFeeRepository
    {
        Task<IReadOnlyList<AreaShippingFee>> GetAllAsync();
        Task<AreaShippingFee?> GetByIdAsync(int id);
        Task<AreaShippingFee?> GetByAreaAsync(string area);
        Task AddAsync(AreaShippingFee areaShippingFee);
        void Update(AreaShippingFee areaShippingFee);
        void Delete(AreaShippingFee areaShippingFee);
        Task<int> SaveChangesAsync();
    }
}
