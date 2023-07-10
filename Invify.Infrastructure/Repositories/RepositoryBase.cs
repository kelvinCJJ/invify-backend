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


        public async Task<bool> CheckForDuplicateAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AnyAsync(expression);
        }

        public async Task<Response> CreateAsync(T entity)
        {
            try { 
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
                return new Response { Success = true, Message = entity.GetType().Name + " created successfully" };
            } catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }

        }
        public async Task<Response> UpdateAsync(T entity)
        {
            try
            {
                _context.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return new Response { Success = true, Message = entity.GetType().Name + " updated successfully" };
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
                return new Response { Success = true, Message = entity.GetType().Name + " deleted successfully" };
            } catch (Exception ex)
            {
                return new Response { Success = false, Message = ex.Message };
            }
        }


    }
}
