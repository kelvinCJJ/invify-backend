using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Analytics
{
    public class ProductSaleForecast
    {
        [ColumnName("Score")]
        public float Quantity { get; set; }
        //[ColumnName("Price")]
        //public float Price { get; set; }
    }
}
