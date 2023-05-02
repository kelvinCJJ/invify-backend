using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<Product>> FindAllAsync()
        {
            return await _context.Set<Product>().Include(p => p.Category).ToListAsync();
        }

        public async Task<Response> CreateAsync(Product product)
        {
            try
            {
                if(product.CategoryId == -1)
                {
                    product.CategoryId = null;
                }
                await _context.Set<Product>().AddAsync(product);
                await _context.SaveChangesAsync();
                return new Response { Success = true, Message = product.GetType().Name + " created successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

        }

        public async Task<Response> UpdateProductAsync(Product product)
        {
            try
            {
                if (product.CategoryId == -1)
                {
                    product.CategoryId = null;
                }
                _context.Attach(product);
                _context.Entry(product).State = EntityState.Modified;
                //_context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

                return new Response { Success = true, Message = product.GetType().Name + " updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }
    }
}
