using Invify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Interfaces
{
    public interface IPurchaseDetailRepository
    {
        Task<List<PurchaseDetail>> GetAllAsync();
        Task<PurchaseDetail> GetByIdAsync(int id);
        Task AddAsync(PurchaseDetail purchaseDetail);
        Task UpdateAsync(PurchaseDetail purchaseDetail);
        Task DeleteAsync(PurchaseDetail purchaseDetail);
    }
}
