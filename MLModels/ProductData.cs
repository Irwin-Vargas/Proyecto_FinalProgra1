using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Proyecto_FinalProgra1.MLModels
{
    public class ProductData
    {
        [LoadColumn(0)]
        public float Ventas { get; set; }

        [LoadColumn(1)]
        public bool EsPopular { get; set; }
    }
}