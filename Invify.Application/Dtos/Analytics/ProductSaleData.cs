using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Analytics
{
    public class ProductSaleData
    {
        [LoadColumn(0)]
        public float ProductId { get; set; }
        
        [LoadColumn(1)]
        public float Quantity { get; set; }
        
        [LoadColumn(2)]
        public float Price { get; set; }

        //[LoadColumn(3)]
        //public DateTime SaleDate { get; set; }
    }
}
