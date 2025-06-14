using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using System.IO;
using Proyecto_FinalProgra1.MLModels;

namespace Proyecto_FinalProgra1.MLModels
{
    public class ProductPopularityModelBuilder
    {
        private static readonly string modelPath = Path.Combine(Environment.CurrentDirectory, "MLModels", "product_popularity_model.zip");

        public static void TrainAndSaveModel(string dataPath)
        {
            var context = new MLContext();

            var data = context.Data.LoadFromTextFile<ProductData>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = context.Transforms.Concatenate("Features", nameof(ProductData.AverageRating), nameof(ProductData.ReviewCount), nameof(ProductData.TotalOrders))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(data);

            context.Model.Save(model, data.Schema, modelPath);
        }

        public static ProductPrediction Predict(ProductData input)
        {
            var context = new MLContext();

            ITransformer model = context.Model.Load(modelPath, out var schema);

            var predictionEngine = context.Model.CreatePredictionEngine<ProductData, ProductPrediction>(model);

            return predictionEngine.Predict(input);
        }
    }
}