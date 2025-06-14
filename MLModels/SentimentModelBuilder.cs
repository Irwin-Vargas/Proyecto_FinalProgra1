using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;

namespace Proyecto_FinalProgra1.MLModels
{
    public class SentimentData
    {
        [LoadColumn(0)] public string Text { get; set; }
        [LoadColumn(1)] public bool Label { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    public static class SentimentModelBuilder
    {
        private static string dataPath = "Data/sentiment-data.tsv";
        private static string modelPath = "MLModels/sentimentModel.zip";

        public static void TrainAndSaveModel()
        {
            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: false);
            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);

            mlContext.Model.Save(model, split.TrainSet.Schema, modelPath);
        }
    }
}