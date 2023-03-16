using Invify.Domain.Entities;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;   
        public CategoryRepository(ApplicationDbContext applicationDbContext, ILogger<CategoryRepository> logger)
        {
            _context = applicationDbContext;
            _logger = logger;
        }

        public Task<Category> AddCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task<Category> DeleteCategory(int id)
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

        public Task<IEnumerable<Category>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetCategory(int id)
        {
            throw new NotImplementedException();
        }

       

        public Task<Category> UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
