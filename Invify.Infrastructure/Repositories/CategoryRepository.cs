using Invify.Domain.Entities;
using Invify.Domain.Interfaces;
using Invify.Infrastructure.Configuration;
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
        public Task AddAsync(Category category)
        {
            //Create a new category
            _context.Category.Add(category);
            //Save the changes
            _context.SaveChanges();
            //Return the task
            return Task.CompletedTask;

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
