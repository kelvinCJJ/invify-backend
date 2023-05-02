using Invify.Domain.Entities;
using Invify.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> FindAllAsync();
        Task<Response> CreateAsync(Product product);
        Task<Response> UpdateAsync(Product product);
        Task<Response> DeleteAsync(Product product);
    }
}
