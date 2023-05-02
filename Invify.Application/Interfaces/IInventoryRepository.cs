using Invify.Domain.Entities;
using Invify.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Interfaces
{
    public interface IInventoryRepository : IRepositoryBase<Inventory>
    {
        Task<IEnumerable<Inventory>> FindAllAsync();
        Task<Response> CreateAsync(Inventory inventory);
        Task<Response> UpdateAsync(Inventory inventory);
        Task<Response> DeleteAsync(Inventory inventory);
    }
}
