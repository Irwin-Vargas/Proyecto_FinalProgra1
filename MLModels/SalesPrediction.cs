using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Proyecto_FinalProgra1.MLModels
{
    public class SalesPrediction
{
    [ColumnName("Score")]
    public float PredictedQuantity { get; set; }
}
}