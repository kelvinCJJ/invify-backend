using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Domain.Entities
{
    public class StockTake : BaseEntity
    {
        public int ProductId { get; set; }
        public int TakenQuantity { get; set;}
    }
}
