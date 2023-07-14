namespace Invify.API.Helpers
{
    public class ForecastHelper
    {
        public List<float> GetFilteredForecastedValues(float[] forecastedValues, float[] upperBoundValues)
        {
            var filteredValues = new List<float>();

            for (int i = 0; i < forecastedValues.Length; i++)
            {
                if (forecastedValues[i] >= 0 && forecastedValues[i] <= upperBoundValues[i])
                {
                    filteredValues.Add(forecastedValues[i]);
                }
            }

            return filteredValues;
        }

        public (float[] totalQuantity, float[] upperBoundTotalQuantity, float[] lowerBoundTotalQuantity) GetNonNegativeForecastedValues(float[] forecastedValues, float[] upperBoundValues, float[] lowerBoundValues)
        {
            // Find the minimum lower bound value
            float minLowerBoundValue = lowerBoundValues.Min();

            // Calculate the offset needed to make all lower bound values non-negative
            float offset = minLowerBoundValue < 0 ? -minLowerBoundValue : 0;

            // Create new arrays for the non-negative forecasted values and bounds
            float[] nonNegativeForecastedValues = new float[forecastedValues.Length];
            float[] nonNegativeUpperBoundValues = new float[upperBoundValues.Length];
            float[] nonNegativeLowerBoundValues = new float[lowerBoundValues.Length];

            // Apply the offset to the forecasted values and bounds
            for (int i = 0; i < forecastedValues.Length; i++)
            {
                nonNegativeForecastedValues[i] = forecastedValues[i] + offset;
                nonNegativeUpperBoundValues[i] = upperBoundValues[i] + offset;
                nonNegativeLowerBoundValues[i] = lowerBoundValues[i] + offset;
            }

            return (nonNegativeForecastedValues, nonNegativeUpperBoundValues, nonNegativeLowerBoundValues);
        }
    }
}
