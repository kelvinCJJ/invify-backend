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
    public class PurchaseRepository : RepositoryBase<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }

        public async Task<IEnumerable<Purchase>> FindAllAsync()
        {
            return await _context.Set<Purchase>().Include(p => p.Product).ToListAsync();
        }

        public async Task<Response> CreateAsync(Purchase purchase)
        {
            try
            {
                await _context.Set<Purchase>().AddAsync(purchase);
                await _context.SaveChangesAsync();
                return new Response { Success = true, Message = purchase.GetType().Name + " created successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

        }

        public async Task<Response> UpdateAsync(Purchase purchase)
        {
            try
            {
                _context.Attach(purchase);
                _context.Entry(purchase).State = EntityState.Modified;
                //_context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

                return new Response { Success = true, Message = purchase.GetType().Name + " updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }
    }
}
