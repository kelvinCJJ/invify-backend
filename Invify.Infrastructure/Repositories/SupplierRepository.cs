using Invify.Domain.Entities;
using Invify.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        public Task AddAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Supplier>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Supplier> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task ISupplierRepository.DeleteAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }

        Task ISupplierRepository.UpdateAsync(Supplier supplier)
        {
            throw new NotImplementedException();
        }
    }
}
