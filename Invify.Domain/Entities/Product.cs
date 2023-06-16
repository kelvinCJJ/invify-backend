using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string SKU { get; set; } 
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; } 
        public int Quantity { get; set; } = 0;
        public int RestockLevel { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
    }

}
