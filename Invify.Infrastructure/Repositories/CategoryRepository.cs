using Invify.Domain.Entities;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext applicationDbContext)
            : base(applicationDbContext)
        {
        }
        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            return await FindAllAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            var category = await FindByConditionAsync(c => c.Name.Equals(name));
            if(category == null)
            {
                return null;
            }
            return category.FirstOrDefault();
        }

        //public async Task CreateCategoryAsync(Category category)
        //{
        //   await CreateAsync(category); 
        //}

        //public async Task UpdateCategoryAsync(Category category)
        //{
        //    await UpdateAsync(category);
        //}
        //public async Task DeleteCategoryAsync(int id)
        //{ 
        //    var category = await FindByConditionAsync(c => c.Id.Equals(id));
        //    await DeleteAsync(category.FirstOrDefault());
        //}





    }
}
