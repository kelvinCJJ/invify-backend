using Invify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<List<Purchase>> GetAllAsync();
        Task<Purchase> GetByIdAsync(int id);
        Task AddAsync(Purchase purchase);
        Task UpdateAsync(Purchase purchase);
        Task DeleteAsync(Purchase purchase);
    }
}
