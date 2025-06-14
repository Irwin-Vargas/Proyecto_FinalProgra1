using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Proyecto_FinalProgra1.MLModels
{
    public class ProductPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool EsPopular { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}