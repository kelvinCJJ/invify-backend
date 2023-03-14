using Invify.Domain.Entities;
using Invify.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task AddAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
