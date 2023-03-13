using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Entities
{
    public class PurchaseDetail : BaseEntity
    {
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        public int ProductId { get; set; }
        public List<Product> Products { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
