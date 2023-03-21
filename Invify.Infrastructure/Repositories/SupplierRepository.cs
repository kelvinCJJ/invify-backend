using Invify.Domain.Entities;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(ApplicationDbContext applicationDbContext)
        : base(applicationDbContext)
        {
        }
    }
}
