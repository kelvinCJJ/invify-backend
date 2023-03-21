using Invify.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Services
{
    public class IRepositoryService
    {
        private IRepositoryWrapper _repository;

        public IRepositoryService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }
    }
}
