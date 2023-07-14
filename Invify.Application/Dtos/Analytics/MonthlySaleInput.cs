using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Analytics
{
    public class MonthlySaleInput
    {
        //Datetime of the sale
        [LoadColumn(0)]
        [ColumnName(@"SaleDate")]
        public DateTime SaleDate { get; set; }

        // The year of the sale
        [LoadColumn(1)]
        [ColumnName(@"Year")]
        public float Year { get; set; }

        // The month of the sale
        [LoadColumn(2)]
        [ColumnName(@"Month")]
        public float Month { get; set; }

        // The total quantity of sales
        [LoadColumn(3)]
        [ColumnName(@"TotalQuantity")]
        public float TotalQuantity { get; set; }
        

    }
}
