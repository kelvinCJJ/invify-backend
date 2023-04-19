using Invify.Dtos;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Invify.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ApplicationDbContext _context { get; set; }
        public RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<T>> FindAllAsync()
        {
           return await _context.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
        }
        public async Task<Response> CreateAsync(T entity)
        {
            try { 
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
                return new Response { Success = true, Message = entity + "Created successfully" };
            } catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

        }
        public async Task<Response> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                await _context.SaveChangesAsync();

                return new Response { Success = true, Message = entity + "Updated successfully" };
            }
            catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }
        public async Task<Response> DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return new Response { Success = true, Message = entity + "Deleted successfully" };
            } catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }


    }
}
