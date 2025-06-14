using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using System.IO;

namespace Proyecto_FinalProgra1.MLModels
{
    public static class ProductPopularityModelBuilder
    {
        private static readonly string modelPath = Path.Combine("MLModels", "product_popularity_model.zip");

        public static void TrainModel(IEnumerable<(string Nombre, int Ventas)> datos, int umbral = 10)
        {
            var mlContext = new MLContext();

            var trainingData = datos.Select(d => new ProductData
            {
                Ventas = d.Ventas,
                EsPopular = d.Ventas >= umbral
            });

            var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms.Conversion
                .MapValueToKey("Label", nameof(ProductData.EsPopular))
                .Append(mlContext.Transforms.Concatenate("Features", nameof(ProductData.Ventas)))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());

            var model = pipeline.Fit(dataView);

            mlContext.Model.Save(model, dataView.Schema, modelPath);
        }

        public static PredictionEngine<ProductData, ProductPrediction> LoadPredictionEngine()
        {
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(modelPath, out var schema);
            return mlContext.Model.CreatePredictionEngine<ProductData, ProductPrediction>(model);
        }
    }
}