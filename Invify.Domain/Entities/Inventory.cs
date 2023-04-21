using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Entities
{
    public class Inventory : BaseEntity
    {     
        public int Quantity { get; set; }
        public int RestockLevel { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
