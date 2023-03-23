using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Invify.Domain.Entities;

namespace Invify.Interfaces
{
    public interface ICategoryRepository : IRepositoryBase<Category>
    {
        Task<IEnumerable<Category>> GetAllCategoryAsync();
        Task<Category> GetCategoryByNameAsync(string name);
        //Task CreateCategoryAsync(Category category);
        //Task UpdateCategoryAsync(Category category);
        //Task DeleteCategoryAsync(int id);

    }
}
