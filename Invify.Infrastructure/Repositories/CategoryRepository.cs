using Invify.Domain.Entities;
using Invify.Infrastructure.Configuration;
using Invify.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Infrastructure.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext applicationDbContext)
            :base(applicationDbContext)
        {
        }

    }
}
