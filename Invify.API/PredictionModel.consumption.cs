﻿// This file was auto-generated by ML.NET Model Builder.

using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.ML.Transforms.TimeSeries;

namespace Invify_Application
{
    public partial class PredictionModel
    {
        /// <summary>
        /// model input class for PredictionModel.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [LoadColumn(2)]
            [ColumnName(@"TotalSales")]
            public float TotalSales { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for PredictionModel.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"TotalSales")]
            public float[] TotalSales { get; set; }

            [ColumnName(@"TotalSales_LB")]
            public float[] TotalSales_LB { get; set; }

            [ColumnName(@"TotalSales_UB")]
            public float[] TotalSales_UB { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath(@"C:\Users\kelvin\source\repos\kelvinCJJ\invify-backend\Invify.API\PredictionModel.mlnet");

        public static readonly Lazy<TimeSeriesPredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<TimeSeriesPredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput? input = null, int? horizon = null)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input, horizon);
        }

        private static TimeSeriesPredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var schema);
            return mlModel.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
        }
    }
}
