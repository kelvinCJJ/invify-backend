using Invify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Interfaces
{
    public interface IInventoryRepository
    {
        Task<List<Inventory>> GetAllAsync();
        Task<Inventory> GetByIdAsync(int id);
        Task AddAsync(Inventory inventory);
        Task UpdateAsync(Inventory inventory);
        Task DeleteAsync(Inventory inventory);
    }
}
