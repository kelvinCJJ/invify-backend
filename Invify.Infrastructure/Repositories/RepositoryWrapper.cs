using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private ApplicationDbContext _context;
        private ICategoryRepository _category;
        private IInventoryRepository _inventory;
        private IProductRepository _product;
        private IPurchaseRepository _purchase;
        private ISaleRepository _sale;
        private ISupplierRepository _supplier;
        private IStockTakeRepository _stocktake;

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_context);
                }

                return _category;
            }
        }

        public IInventoryRepository Inventory
        {
            get
            {
                if (_inventory == null)
                {
                    _inventory = new InventoryRepository(_context);
                }

                return _inventory;
            }
        }

        public IProductRepository Product
        {
            get
            {
                if (_product == null)
                {
                    _product = new ProductRepository(_context);
                }

                return _product;
            }
        }


        public IPurchaseRepository Purchase
        {
            get
            {
                if (_purchase == null)
                {
                    _purchase = new PurchaseRepository(_context);
                }

                return _purchase;
            }
        }

        public ISaleRepository Sale
        {
            get
            {
                if (_sale == null)
                {
                    _sale = new SaleRepository(_context);
                }

                return _sale;
            }
        }

        public ISupplierRepository Supplier
        {
            get
            {
                if (_supplier == null)
                {
                    _supplier = new SupplierRepository(_context);
                }

                return _supplier;
            }
        }

        public IStockTakeRepository StockTake
        {
            get
            {
                if (_stocktake == null)
                {
                    _stocktake = new StockTakeRepository(_context);
                }

                return _stocktake;
            }
        }



        public RepositoryWrapper(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }

    }
}
