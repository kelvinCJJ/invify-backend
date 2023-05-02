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
    public class StockTakeRepository : RepositoryBase<StockTake>, IStockTakeRepository
    {
        public StockTakeRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {

        }

        public async Task<List<StockTake>> FindAllAsync()
        {
            return await _context.Set<StockTake>().ToListAsync();
        }
        public async Task<List<StockTake>> FindByConditionAsync(Expression<Func<StockTake, bool>> expression)
        {

            return await _context.Set<StockTake>().Where(expression).AsNoTracking().ToListAsync();
        }


        public async Task<Response> UpdateStockAsync(StockTake sale)
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
