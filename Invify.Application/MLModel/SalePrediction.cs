using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Data.SqlClient;

namespace Invify.MLModel
{
    public class SalePrediction
    {
        public SalePrediction(DateTime dateTime, int quantity)
        {
            Datetime = dateTime;
            Quantity = quantity;
        }

        [LoadColumn(0)]
        public DateTime Datetime { get; set; }
        [LoadColumn(1)]
        public int Quantity { get; set; }

        public class Prediction
        {
            [ColumnName("Month-Year")]
            public DateTime Datetime { get; set; }
            [ColumnName("Quantity")]
            public int Quantity { get; set; }
        }
    }
}
