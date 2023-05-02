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
    public interface IStockTakeRepository : IRepositoryBase<StockTake>
    {
        Task<List<StockTake>> FindAllAsync();
        Task<List<StockTake>> FindByConditionAsync(Expression<Func<StockTake, bool>> expression);
        Task<Response> UpdateAsync(StockTake stocktake);
    }
}
