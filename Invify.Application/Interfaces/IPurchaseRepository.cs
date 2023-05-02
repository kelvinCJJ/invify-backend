using Invify.Domain.Entities;
using Invify.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Interfaces
{
    public interface IPurchaseRepository : IRepositoryBase<Purchase>
    {
        Task<IEnumerable<Purchase>> FindAllAsync();
        Task<Response> CreateAsync(Purchase purchase);
        Task<Response> UpdateAsync(Purchase purchase);
        Task<Response> DeleteAsync(Purchase purchase);
    }
}
