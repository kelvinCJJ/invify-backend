using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Interfaces
{
    public interface IRepositoryWrapper
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IPurchaseRepository Purchase { get; }
        ISaleRepository Sale { get; }
        ISupplierRepository Supplier { get; }
        IStockTakeRepository StockTake { get; }
        Task SaveAsync();
    }
}
