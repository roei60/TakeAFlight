using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakeAFlight.Models.ML
{
    public class DestPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabels;
    }
}
