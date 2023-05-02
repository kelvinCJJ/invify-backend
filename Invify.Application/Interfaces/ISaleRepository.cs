using Invify.Domain.Entities;
using Invify.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Interfaces
{
    public interface ISaleRepository : IRepositoryBase<Sale>
    {
        Task<List<Sale>> FindAllAsync();
        Task<List<Sale>> FindByConditionAsync(Expression<Func<Sale, bool>> expression);
        Task<Response> CreateAsync(Sale sale);
        Task<Response> UpdateAsync(Sale sale);
        Task<Response> DeleteAsync(Sale sale);
    }
}
