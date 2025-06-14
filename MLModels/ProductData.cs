using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Proyecto_FinalProgra1.MLModels
{
    public class ProductData
    {
        [LoadColumn(0)] public float ProductId { get; set; }
        [LoadColumn(1)] public float AverageRating { get; set; }
        [LoadColumn(2)] public float ReviewCount { get; set; }
        [LoadColumn(3)] public float TotalOrders { get; set; }
        [LoadColumn(4)] public bool Label { get; set; } // Popularidad
    }
}