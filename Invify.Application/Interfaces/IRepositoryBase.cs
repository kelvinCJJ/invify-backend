using System.Linq.Expressions;

namespace Invify.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<List<T>> FindAllAsync();
        Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
