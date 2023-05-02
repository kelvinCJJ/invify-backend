using Invify.Domain.Entities;
using Invify.Dtos;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class SaleRepository : RepositoryBase<Sale>, ISaleRepository
    {
        public SaleRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }

        public async Task<List<Sale>> FindAllAsync()
        {
            return await _context.Set<Sale>().Include(s => s.Product).ToListAsync();
        }

        public async Task<List<Sale>> FindByConditionAsync(Expression<Func<Sale, bool>> expression)
        {

            return await _context.Set<Sale>().Where(expression).Include(s => s.Product).AsNoTracking().ToListAsync();
        }

        public async Task<Response> CreateAsync(Sale sale)
        {
            try
            {
                await _context.Set<Sale>().AddAsync(sale);
                await _context.SaveChangesAsync();
                return new Response { Success = true, Message = sale.GetType().Name + " created successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

        }

        public async Task<Response> UpdateProductAsync(Sale sale)
        {
            try
            {
                _context.Attach(sale);
                _context.Entry(sale).State = EntityState.Modified;
                //_context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

                return new Response { Success = true, Message = sale.GetType().Name + " updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }
    }
}
