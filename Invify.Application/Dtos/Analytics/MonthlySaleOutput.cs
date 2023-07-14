using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invify.Dtos.Analytics
{
    public class MonthlySaleOutput
    {
        // The predicted total quantity of sales
        [ColumnName(@"TotalQuantity")]
        public float[] TotalQuantity { get; set; }

        // The upper bound of the 95% confidence interval for total quantity of sales
        [ColumnName(@"UpperBoundTotalQuantity")]
        public float[] UpperBoundTotalQuantity { get; set; }

        // The lower bound of the 95% confidence interval for total quantity of sales
        [ColumnName(@"LowerBoundTotalQuantity")]
        public float[] LowerBoundTotalQuantity { get; set; }

    }
}
