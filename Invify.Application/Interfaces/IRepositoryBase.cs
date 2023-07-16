using Invify.Dtos;
using System.Linq.Expressions;

namespace Invify.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<List<T>> FindAllAsync();
        Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<int> TotalCountAsync();
        Task<bool> CheckForDuplicateAsync(Expression<Func<T, bool>> expression);
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
    }
}
