using Invify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Interfaces
{
    public interface ISaleRepository
    {
        Task<List<Sale>> GetAllAsync();
        Task<Sale> GetByIdAsync(int id);
        Task AddAsync(Sale sale);
        Task UpdateAsync(Sale sale);
        Task DeleteAsync(Sale sale);
    }
}
